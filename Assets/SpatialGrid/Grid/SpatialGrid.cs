﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using UnityEditor;

public class SpatialGrid : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public int CurrentAgentsInsideGrid => RecursiveWalker(transform).Select(transform1 => transform1.GetComponent<GridEntity>()).Count(entity => entity != null);
    #region Variables
    //punto de inicio de la grilla en X
    public float x;
    //punto de inicio de la grilla en Z
    public float y;
    //ancho de las celdas
    public float cellWidth;
    //alto de las celdas
    public float cellHeight;
    //cantidad de columnas (el "ancho" de la grilla)
    public int width;
    //cantidad de filas (el "alto" de la grilla)
    public int height;

    //ultimas posiciones conocidas de los elementos, guardadas para comparación.
    private Dictionary<GridEntity, Tuple<int, int>> lastPositions;
    //los "contenedores"
    private HashSet<GridEntity>[,] buckets;
    
    //el valor de posicion que tienen los elementos cuando no estan en la zona de la grilla.
    /*
     Const es implicitamente statica
     const tengo que ponerle el valor apenas la declaro, readonly puedo hacerlo en el constructor.
     Const solo sirve para tipos de dato primitivos.
     */
    protected readonly Tuple<int, int> Outside = Tuple.Create(-1, -1);
    //Una colección vacía a devolver en las queries si no hay nada que devolver
    private readonly GridEntity[] Empty = Array.Empty<GridEntity>();
    #endregion

    #region FUNCIONES
    public void Initialize()
    {
        lastPositions = new Dictionary<GridEntity, Tuple<int, int>>();
        buckets = new HashSet<GridEntity>[width, height];

        //creamos todos los hashsets
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                buckets[i, j] = new HashSet<GridEntity>();
            }
        }

        //P/alumnos: por que no usamos OfType<>() despues del RecursiveWalker() aca?
        var entities = RecursiveWalker(transform)
            .Select(entityTransform => entityTransform.GetComponent<GridEntity>())
            .Where(gridEntity => gridEntity != null);

        foreach (var e in entities)
        {
            RegisterEntity(e);
        }
    }

    public void Terminate()
    {
        var ents = RecursiveWalker(transform).Select(x => x.GetComponent<GridEntity>()).Where(x => x != null);
        
        foreach (var e in ents)
        {
            UnRegisterEntity(e);
        }
    }

    private void OnDestroy() => Terminate();
    public void RegisterEntity(GridEntity gridEntity)
    {
        gridEntity.OnUpdatePosition += UpdateEntity;
        
        UpdateEntity(gridEntity);
    }
    
    public void UnRegisterEntity(GridEntity gridEntity)
    {
        gridEntity.OnUpdatePosition -= UpdateEntity;
    }
    private void UpdateEntity(GridEntity entity)
    {
        var lastPos = lastPositions.TryGetValue(entity, out var position) ? position : Outside;
        var currentPos = GetPositionInGrid(entity.gameObject.transform.position);

        //Misma posición, no necesito hacer nada
        if (lastPos.Equals(currentPos)) return;

        //Lo "sacamos" de la posición anterior
        if (IsInsideGrid(lastPos))
            buckets[lastPos.Item1, lastPos.Item2].Remove(entity);

        //Lo "metemos" a la celda nueva, o lo sacamos si salio de la grilla
        if (IsInsideGrid(currentPos))
        {
            buckets[currentPos.Item1, currentPos.Item2].Add(entity);
            lastPositions[entity] = currentPos;
        }
        else
            lastPositions.Remove(entity);
    }

    public IEnumerable<GridEntity> Query(Vector3 aabbFrom, Vector3 aabbTo, Func<Vector3, bool> filterByPosition)
    {
        var from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), Mathf.Min(aabbFrom.y, aabbTo.y),0);
        var to = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), Mathf.Max(aabbFrom.y, aabbTo.y, 0));

        var fromCoord = GetPositionInGrid(from);
        var toCoord = GetPositionInGrid(to);

        //¡Ojo que clampea a 0,0 el Outside! TODO: Checkear cuando descartar el query si estan del mismo lado
        fromCoord = Tuple.Create(Utility.Clampi(fromCoord.Item1, 0, width), Utility.Clampi(fromCoord.Item2, 0, height));
        toCoord = Tuple.Create(Utility.Clampi(toCoord.Item1, 0, width), Utility.Clampi(toCoord.Item2, 0, height));

        if (!IsInsideGrid(fromCoord) && !IsInsideGrid(toCoord)) return Empty;
            
        // Creamos tuplas de cada celda
        var cols = Generate(fromCoord.Item1, x => x + 1).TakeWhile(x => x < width && x <= toCoord.Item1);
        var rows = Generate(fromCoord.Item2, y => y + 1).TakeWhile(y => y < height && y <= toCoord.Item2);
        var cells = cols.SelectMany(col => rows.Select(row => Tuple.Create(col, row)));
        var availableGridEntities = cells.SelectMany(cell => buckets[cell.Item1, cell.Item2]).Where(entity => entity.IsDestroying == false);
        
        // Iteramos las que queden dentro del criterio
        return availableGridEntities.Where(e => {
                                Vector3 position;
                                return from.x <= (position = e.transform.position).x && position.x <= to.x && from.y <= position.y && position.y <= to.y;})
                           .Where(entity => filterByPosition(entity.transform.position) && entity.gameObject.activeSelf && entity.IsDestroying == false);
    }

    public Tuple<int, int> GetPositionInGrid(Vector3 pos)
    {
        //quita la diferencia, divide segun las celdas y floorea
        return Tuple.Create(Mathf.FloorToInt((pos.x - x) / cellWidth),
                            Mathf.FloorToInt((pos.y - y) / cellHeight));
    }

    public bool IsInsideGrid(Tuple<int, int> position)
    {
        //si es menor a 0 o mayor a width o height, no esta dentro de la grilla
        return 0 <= position.Item1 && position.Item1 < width &&
            0 <= position.Item2 && position.Item2 < height;
    }



    #region GENERATORS
    private static IEnumerable<Transform> RecursiveWalker(Transform parent)
    {
        foreach (Transform child in parent)
        {
            foreach (Transform grandchild in RecursiveWalker(child))
                yield return grandchild;
            yield return child;
        }
    }

    IEnumerable<T> Generate<T>(T seed, Func<T, T> mutate)
    {
        T accum = seed;
        while (true)
        {
            yield return accum;
            accum = mutate(accum);
        }
    }
    #endregion

    #endregion

    #region GRAPHIC REPRESENTATION
    public bool AreGizmosShutDown;
    public bool activatedGrid;
    public bool showLogs = true;
    private void OnDrawGizmos()
    {
        var rows = Generate(y, curr => curr + cellHeight)
                .Select(row => Tuple.Create(new Vector3(x, row, 0),
                                            new Vector3(x + cellWidth * width, row, 0)));

        //equivalente de rows
        /*for (int i = 0; i <= height; i++)
        {
            Gizmos.DrawLine(new Vector3(x, 0, z + cellHeight * i), new Vector3(x + cellWidth * width,0, z + cellHeight * i));
        }*/

        var cols = Generate(x, curr => curr + cellWidth)
                   .Select(col => Tuple.Create(new Vector3(col, y, 0f), new Vector3(col, y + cellHeight * height, 0)));

        var allLines = rows.Take(width + 1).Concat(cols.Take(height + 1));

        foreach (var elem in allLines)
        {
            Gizmos.DrawLine(elem.Item1, elem.Item2);
        }

        if (buckets == null || AreGizmosShutDown) return;

        var originalCol = GUI.color;
        GUI.color = Color.red;
        if (!activatedGrid)
        {
            IEnumerable<GridEntity> allElems = Enumerable.Empty<GridEntity>();
            foreach(var elem in buckets)
                allElems = allElems.Concat(elem);

            int connections = 0;
            foreach (var ent in allElems)
            {
                foreach(var neighbour in allElems.Where(x => x != ent))
                {
                    Gizmos.DrawLine(ent.transform.position, neighbour.transform.position);
                    connections++;
                }
                if(showLogs)
                    Debug.Log("tengo " + connections + " conexiones por individuo");
                connections = 0;
            }
        }
        else
        {
            int connections = 0;
            
            foreach (var elem in buckets)
            {
                foreach(var ent in elem)
                {
                    int number = elem.Count();
                    
                    var numberStyle = new GUIStyle
                    {
                        normal =
                        {
                            textColor = Color.white
                        },
                        fontSize = 20
                    };

                    var numberText = number.ToString();
                    var position = ent.transform.position;
                    
                    // Handles.Label(position, numberText, numberStyle);
                    
                    foreach (var n in elem.Where(x => x != ent))
                    {
                        Gizmos.DrawLine(ent.transform.position, n.transform.position);
                        connections++;
                    }
                    if(showLogs)
                        Debug.Log("tengo " + connections + " conexiones por individuo");
                    connections = 0;
                }
            }
        }

        GUI.color = originalCol;
        showLogs = false;
    }
    #endregion
}
