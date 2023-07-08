using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
public enum DeltaTimeType { DeltaTime, FixedDeltaTime, UnscaledDeltaTime, FixedUnscaledDeltaTime }

public static class Extensions 
{
    #region Array
    public static T Last<T>(this T[] source) => source[source.Length - 1];
    public static T First<T>(this T[] source) => source[0];
    #endregion
    #region GameObjects
    public static T GetOrAddComponent<T>(this GameObject source) where T : Component
    {
        var component = source.GetComponent<T>();
        
        if (component == null) component = source.AddComponent<T>();
            
        return component;
    }
    #endregion
    #region List
    public static T Last<T>(this List<T> source) => source[source.Count - 1];
    public static T First<T>(this List<T> source) => source[0];
    public static int InsertSorted<T>(this List<T> source, T nElement)
    {
        var index = source.BinarySearch(nElement);
        if (index < 0) index = ~index;
        source.Insert(index, nElement);
        return index;
    }
    public static T FindLowest<T>(this List<T> source, Func<T, float> testing)
    {
        float lowest = int.MaxValue;
        var current = default(T);
        
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (!(testing.Invoke(source[i]) < lowest)) continue;
            
            lowest = testing.Invoke(source[i]);
            current = source[i];
        }
        return current;
    }
    public static T FindHighest<T>(this List<T> source, Func<T, float> testing)
    {
        float highest = int.MinValue;
        var current = default(T);
        
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (!(testing.Invoke(source[i]) > highest)) continue;
            
            highest = testing.Invoke(source[i]);
            current = source[i];
        }
        return current;
    }
    public static List<T> ExtractAll<T>(this List<T> source, Func<T, bool> validation)
    {
        var values = new List<T>();
        
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (!validation.Invoke(source[i])) continue;
            
            values.Add(source[i]);
            source.RemoveAt(i);
        }
        return values;
    }
    public static T Take<T>(this List<T> source, int index, bool removePosition = true) 
    {
        var value = source[index];
        
        if (removePosition) source.RemoveAt(index);
        else source[index] = default;
            
        return value;
    }
    public static void Shuffle<T>(this List<T> source)
    {
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var j = UnityEngine.Random.Range(0, i);
            (source[i], source[j]) = (source[j], source[i]);
        }
    }
    public static int FindMinIndex<T>(this List<T> source, Func<T, float> function)
    {
        var min = float.MaxValue;
        var current = -1;
        
        for (var i = 0; i < source.Count; i++)
        {
            var value = function.Invoke(source[i]);

            if (!(min > value)) continue;
            
            min = value;
            current = i;
        }
        return current;
    }
    public static T FindMin<T>(this List<T> source, Func<T, float> function) => source[FindMinIndex(source, function)];
    #endregion
    #region Scene
    public static T FindObjectInScene<T>(this Scene scene, bool includeInactive = false) where T : Component
    {
        if (!scene.isLoaded || !scene.IsValid()) throw new Exception("Scene isn't loaded or is Invalid");
        
        var allFound = Object.FindObjectsOfType<T>(includeInactive);
        
        return allFound.FirstOrDefault(found => found.gameObject.scene == scene);
    }
    
    public static T[] FindObjectsInScene<T>(this Scene scene, bool includeInactive = false) where T : Component
    {
        if (!scene.isLoaded || !scene.IsValid()) throw new Exception("Scene isn't loaded or is Invalid");

        var allFound = Object.FindObjectsOfType<T>(includeInactive);

        return allFound.Where(found => found.gameObject.scene == scene).ToArray();
    }
    public static UnityEngine.SceneManagement.Scene[] GetAllOpenedScenes()
    {
        var countLoaded = UnityEngine.SceneManagement.SceneManager.sceneCount;
        
        var loadedScenes = new UnityEngine.SceneManagement.Scene[countLoaded];

        for (var i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
        }
        
        return loadedScenes;
    }
    #endregion
    #region Transform
    public static string GetHierarchyAsString(this Transform source, bool includeSceneName)
    {
        if(includeSceneName!) return source.parent ? $"{source.parent.GetHierarchyAsString(true)}/{source.name}" : $"{source.gameObject.scene.name}/{source.name}";
        return source.parent ? $"{source.parent.GetHierarchyAsString(false)}/{source.name}" : source.name;
    }
    public static void SetGlobalScale (this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        var lossyScale = transform.lossyScale;
        transform.localScale = new Vector3 (globalScale.x/lossyScale.x, globalScale.y/lossyScale.y, globalScale.z/lossyScale.z);
    }
    #endregion
    #region Misc

        public const float GoldenRatio = 1.61803398875f;

    public static List<Transform> GetAllChildren(this Transform source)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in source)
            children.Add(child);
        return children;
    }
   
    public static void SetLayerRecursively(this GameObject source, int layer)
    {
        foreach (var item in source.transform.GetComponentsInChildren<Transform>(true))
            item.gameObject.layer = layer;
    }
    public static void PlayCoroutine(this MonoBehaviour source, ref IEnumerator rutine, Func<IEnumerator> rutineMethod)
    {
        if (!source.isActiveAndEnabled)
            throw new Exception("El objeto está desactivado o el componente disabled.");

        if (rutine != null)
            source.StopCoroutine(rutine);

        rutine = rutineMethod();
        source.StartCoroutine(rutine);
    }

    public static bool CanSee(this Camera cam, Vector3 pos, Vector3 boundSize) => CanSee(cam, new Bounds(pos, boundSize));
    
    public static bool CanSee(this Camera cam, Bounds bounds) 
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
    
    
    
    public static IEnumerator CreateQuickSteppedRutine(this MonoBehaviour source, float duration, DeltaTimeType deltaTime, Action<float> step, Action callback = null)
    {
        Func<float> deltaStep;
        switch (deltaTime)
        {
            default:
            case DeltaTimeType.DeltaTime:
                deltaStep = ()=> Time.deltaTime;
                break;
            case DeltaTimeType.FixedDeltaTime:
                deltaStep = ()=> Time.fixedDeltaTime;
                break;
            case DeltaTimeType.UnscaledDeltaTime:
                deltaStep = ()=> Time.unscaledDeltaTime;
                break;
            case DeltaTimeType.FixedUnscaledDeltaTime:
                deltaStep = ()=> Time.fixedUnscaledDeltaTime;
                break;
        }

        float t = 0;
        do
        {
            step.Invoke(t);
            yield return null;
            t += deltaStep.Invoke() / duration;
        } while (t<1);
        step.Invoke(1);
        callback?.Invoke();
    }

    public static IEnumerator NonStopAnimation(this MonoBehaviour source, DeltaTimeType deltaTime, Action<float> stepAnimation)
    {
        Func<float> deltaStep;
        switch (deltaTime)
        {
            default:
            case DeltaTimeType.DeltaTime:
                deltaStep = ()=> Time.deltaTime;
                break;
            case DeltaTimeType.FixedDeltaTime:
                deltaStep = ()=> Time.fixedDeltaTime;
                break;
            case DeltaTimeType.UnscaledDeltaTime:
                deltaStep = ()=> Time.unscaledDeltaTime;
                break;
            case DeltaTimeType.FixedUnscaledDeltaTime:
                deltaStep = ()=> Time.fixedUnscaledDeltaTime;
                break;
        }

        do
        {
            yield return null;
            stepAnimation.Invoke(deltaStep.Invoke());
        } while (true);
    }

    public static void WaitAndExecute(this MonoBehaviour source, ref IEnumerator rutine, float waitTime, bool isRealTime, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitAndExecuteRutine(waitTime, isRealTime, callback));
    }

    public static IEnumerator WaitAndExecuteRutine(float waitTime, bool isRealTime, Action callback)
    {
        if (isRealTime)
            yield return new WaitForSecondsRealtime(waitTime);
        else
            yield return new WaitForSeconds(waitTime);

        callback.Invoke();
    }
    
    public static void WaitForNextFrameAndExecute(this MonoBehaviour source, ref IEnumerator rutine, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitForNextFrameRoutine(callback));
    }

    public static IEnumerator WaitForNextFrameRoutine(Action callback)
    {
        yield return null;
        callback.Invoke();
    }

    public static void WaitForFixedUpdateAndExecute(this MonoBehaviour source, ref IEnumerator rutine, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitForFixedUpdateRoutine(callback));
    }
    

    public static IEnumerator WaitForFixedUpdateRoutine(Action callback)
    {
        yield return new WaitForFixedUpdate();
        callback.Invoke();
    }
    
    public static void WaitForEndOfFrameAndExecute(this MonoBehaviour source, ref IEnumerator rutine, Action callback)
    {
        if (callback == null)
            Debug.LogError("Callback is null");
        source.PlayCoroutine(ref rutine, () => WaitForEndOfFrameRoutine(callback));
    }
    
    public static IEnumerator WaitForEndOfFrameRoutine(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback.Invoke();
    }

    public static bool IsBetweenInclusive(this int value, Vector2Int minMax)
    {
        return value >= minMax.x && value <= minMax.y;
    }

    public static bool IsBetweenExclusive(this int value, Vector2Int minMax)
    {
        return value > minMax.x && value < minMax.y;
    }
    

    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static Vector2 AsVectorXY(this float degree) => RadianToVector2(degree * Mathf.Deg2Rad);
    
    public static Vector3 AsVectorXZ(this float degree) => RadianToVector3(degree * Mathf.Deg2Rad);

    
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector3 RadianToVector3(float radian)
    {
        return new Vector3(Mathf.Cos(radian), 0,Mathf.Sin(radian));
    }

    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) 
        {  
            n--;
            int k = Random.Range(0, n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }

    public static Vector2 ClampPositionToView(this Camera camera, Vector2 value)
    {
        float hSize = GetHorizontalSize(camera);
        return new Vector2(Mathf.Clamp(value.x, -hSize, hSize), Mathf.Clamp(value.y, -camera.orthographicSize, camera.orthographicSize)) + (Vector2)camera.transform.position;
    }

  
  
    public static bool IsInsideCameraView(this Camera camera, Vector2 value)
    {
        value -= (Vector2)camera.transform.position;
        return Mathf.Abs(value.y) < camera.orthographicSize && Mathf.Abs(value.x) < camera.GetHorizontalSize();
    }

    public static float GetHorizontalSize(this Camera camera)
    {
        return camera.aspect * camera.orthographicSize * 2;
    }

    public static Vector2 GetOrtographicSize(this Camera camera)
    {
        return new Vector2(camera.GetHorizontalSize(), camera.orthographicSize * 2);
    }

    public static bool ContainsInLocalSpace(this BoxCollider2D boxCollider2D, Vector2 worldSpacePoint)
    {
        worldSpacePoint = boxCollider2D.transform.InverseTransformPoint(worldSpacePoint);
        return Mathf.Abs(worldSpacePoint.x) <= boxCollider2D.size.x / 2 && Mathf.Abs(worldSpacePoint.y) <= boxCollider2D.size.y / 2;
    }




    public static Rect SetCenter(this ref Rect rect, Vector2 newPos)
    {
        rect.Set(newPos.x - rect.width / 2, newPos.y - rect.height / 2, rect.width, rect.height);
        return rect;
    }

    public static float GetPointDistanceFromLine(Vector2 lineStart, Vector2 lineEnd, Vector2 targetPoint)=>(targetPoint.x - lineStart.x) * (lineEnd.y-lineStart.y)-(targetPoint.y-lineStart.y)*(lineEnd.x-lineStart.x);
    

    public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {

        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;

        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;

        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x;
            x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x;
            x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y;
            y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y;
            y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//
        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }
        e = Ax * Cy - Ay * Cx;  // beta numerator//
        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }
        // check if they are parallel
        if (f == 0) return false;
        // compute intersection coordinates //
        num = d * Ax; // numerator //

        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

        //    intersection.x = p1.x + (num+offset) / f;
        intersection.x = p1.x + num / f;

        num = d * Ay;

        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
        //    intersection.y = p1.y + (num+offset) / f;
        intersection.y = p1.y + num / f;
        return true;
    }



    public static T GetRandom<T>(this T[] value)
    {
        if (value.Length == 0) throw new Exception("Array is empty");
        return value[Random.Range(0, value.Length)];
    }

    public static T GetRandom<T>(this IReadOnlyList<T> value)
    {
        if (value.Count == 0) throw new Exception("List is empty");

        return value[Random.Range(0, value.Count)];
    }

    public static List<T> GetRandom<T>(this IReadOnlyList<T> value, int count, bool canRepeat)
    {
        if (!canRepeat && count > value.Count)
            Debug.LogError("Count is Bigger than the list, but we cannot repeat values. Configuration is imposible to fulfil");

        var iterationList = new List<T>(value);
        var retValue = new List<T>();

        while (retValue.Count < count)
        {
            var element = iterationList.GetRandom();
            if (!canRepeat) iterationList.Remove(element);
            retValue.Add(element);
        }
        return retValue;
    }

    public static T GetLast<T>(this List<T> value)
    {
        return value[value.Count - 1];
    }

    public static T GetLast<T>(this T[] value)
    {
        return value[value.Length - 1];
    }

    public static T[] GetElements<T>(this T[] value, int[] indexes) where T : Object
    {
        T[] rValue = new T[indexes.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            rValue[i] = value[indexes[i]];
        }
        return rValue;
    }

    public static T[] GetElements<T>(this List<T> value, int[] indexes) where T : Object
    {
        return GetElements(value.ToArray(), indexes);
    }

    public static Vector2[] GetVertex(this BoxCollider2D box)
    {
        Vector2[] retValue = new Vector2[4];
        retValue[0] = new Vector2(box.bounds.min.x - box.edgeRadius, box.bounds.min.y - box.edgeRadius);
        retValue[1] = new Vector2(box.bounds.min.x - box.edgeRadius, box.bounds.max.y + box.edgeRadius);
        retValue[2] = new Vector2(box.bounds.max.x + box.edgeRadius, box.bounds.max.y + box.edgeRadius);
        retValue[3] = new Vector2(box.bounds.max.x + box.edgeRadius, box.bounds.min.y - box.edgeRadius);
        return retValue;
    }

    public static List<List<Vector2>> GetAllPaths(this PolygonCollider2D collider)
    {
        List<List<Vector2>> polygons = new List<List<Vector2>>();
        for (int i = 0; i < collider.pathCount; i++)
        {
            polygons.Add(new List<Vector2>(collider.GetPath(i)));
        }
        return polygons;
    }

    public static void SetPaths(this PolygonCollider2D collider2D, List<List<Vector2>> nPaths)
    {
        collider2D.pathCount = nPaths.Count;
        for (int i = 0; i < collider2D.pathCount; i++)
        {
            collider2D.SetPath(i, nPaths[i].ToArray());
        }
    }
    public static float SnapTo(this float value, float snap)
    {
        return Mathf.Round(value / snap) * snap;
    }
  
    public static Color ColorFromString(this string value)
    {
        return new Color(
            (value.GetHashCode() / 256f * 3) % 1,
            (value.GetHashCode() / 256f * 6) % 1,
            (value.GetHashCode() / 256f * 3) % 1);
    }

    public static float GetAsPercentageBetween(this float value, float floor, float ceil, bool clamp)
    {
        if (clamp)
        {
            if (value > ceil) return 1;
            if (value < floor) return 0;
        }
        return (value - floor) / (ceil - floor);
    }

    public static float GetAsPercentageBetween(this float value, Vector2 range, bool clamp) => GetAsPercentageBetween(value, range.x, range.y, clamp);
    public static float GetAsPercentageBetween(this float value, Vector2Int range, bool clamp) => GetAsPercentageBetween(value, range.x, range.y, clamp);

    public static IList<T> CollectComponents<T>(this IList<GameObject> value) where T : Component
    {
        List<T> l = new List<T>();
        foreach (GameObject gameObject in value)
        {
            T aux = gameObject.GetComponent<T>();
            if (aux != null) l.Add(aux);
        }
        return l;
    }

    public static int NextIndex(this int value, int baseModule)
    {
        return (value + baseModule + 1) % baseModule;
    }

    public static int PreviousIndex(this int value, int baseModule)
    {
        return (value + baseModule - 1) % baseModule;
    } 

    public static void DestroyContent<T>(this List<T> value) where T : Object
    {
        if (value.Count == 0) return;
        for (int i = value.Count-1; i >= 0; i--)
            Object.Destroy(value[i]);
    }
    public static int GetIndexOfClosest(this Transform source, List<Transform> targets)
    {
        float minDistance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = (targets[i].position - source.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        return index;
    }
    
    public static IEnumerable<Type> GetAllSubclassTypes<T>() 
    {
        return from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
            select type;
    }

    public static Color ColorFromString(this string value, bool useAlpha)
    {
        int aux = value.GetHashCode();
        Color c;
        c.b = ((aux) & 0xFF) / 255f;
        c.g = ((aux >> 8) & 0xFF) / 255f;
        c.r = ((aux >> 16) & 0xFF) / 255f;
        c.a = useAlpha ? ((aux >> 24) & 0xFF) / 255f : 1;
        return c;
    }

    public static float Apothem(float radius) => Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(radius * .5f, 2));

    #endregion
    #region Vector2
 
    public static Vector2 GetRotation(this Vector2 v, float degrees)
    {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        var tx = v.x;
        var ty = v.y;
        
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static void Swap(this Vector2 value)=>  (value.x, value.y) = (value.y, value.x);
    public static void Swap(this Vector2Int value)=> (value.x, value.y) = (value.y, value.x);
    public static float Round(this float value, float roundValue) => Mathf.Round(value / roundValue) * roundValue; 
    public static int GetRandom(this Vector2Int value, bool includeBottom = true, bool includeTop = false) => Random.Range(value.x + (includeBottom?0:1), value.y + (includeTop?1:0));
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
    
    public static Vector2 GetDirection(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).normalized;
    }
    public static float GetMagnitude(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).magnitude;
    }
    public static float GetPercentageFor(this Vector2 source, float value, bool clamp) => value.GetAsPercentageBetween(source, clamp);
    public static Vector2 GetDifference(this Vector2 startPos, Vector2 endPos) => (endPos - startPos);

    public static float GetAngle(this Vector2 startPos, Vector2 endPos)
    {
        Vector2 dif = GetDirection(startPos, endPos);
        float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        return angle;
    }
    public static float AsAngle(this Vector2 source)
    {
        Vector2 normalized = source.normalized;
        float angle = Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg;
        return angle;
    }

    public static float GetRandomBetweenXY(this Vector2 value)
    {
        return Mathf.Lerp(value.x, value.y, Random.Range(0, 1f));
    }
    
    public static Vector2 GetRandomBetween(this Vector2 startPos, Vector3 endpos)
    {
        return Vector2.Lerp(startPos, endpos, Random.Range(0, 1f));
    }

    public static bool IsValueBetween(this Vector2 source, float value) => value >= source.x && value <= source.y;

    public static Vector2 GetRandomBetweenAsRect(this Vector2 startPos, Vector3 endpos)
    {
        return new Vector2(Mathf.Lerp(startPos.x, endpos.x, Random.Range(0f, 1f)), Mathf.Lerp(startPos.y, endpos.y, Random.Range(0f, 1f)));
    }
    
    public static float GetSqrMagnitud(this Vector2 startPos, Vector2 endPos)
    {
        return GetDifference(startPos, endPos).sqrMagnitude;
    }

    public static float GetLerp(this Vector2 startPos, float t)
    {
        return Mathf.Lerp(startPos.x, startPos.y, t);
    }
    
    public static float GetLerpUnclamped(this Vector2 startPos, float t)
    {
        return Mathf.LerpUnclamped(startPos.x, startPos.y, t);
    }
    
    public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
    }
    
    public static Vector2 SnapTo(this Vector2 value, float snap)
    {
        Vector2 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        return retValue;
    }
    
    public static Vector2 GetCropping(this Vector2 fromResolution, float toAspect)
    {
        Vector2 dif = Vector2.zero;
        float aspect = Camera.main!.aspect;
        
        if (toAspect > aspect)
        {
            //Debug.Log("Horizontal");
            float targetHeight = fromResolution.x / toAspect;
            dif.y = (fromResolution.y - targetHeight) / 2;
        }
        else
        {
            //Debug.Log("Vertical");
            float targetWidth = fromResolution.y * toAspect;
            dif.x = (fromResolution.x - targetWidth) / 2;
        }
        return dif;
    }
    
    public static Vector2 PerpendicularClockwise(this Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }
    

    #endregion
    #region Vector2Int
    public static int DifferenceXtoY(this Vector2Int value)
    {
        return value.y - value.x;
    }
    public static Vector3Int AsVector3IntXZ(this Vector2Int value)=> new Vector3Int(value.x,0,value.y);

    public static float GetPercentageFor(this Vector2Int source, float value, bool clamp) => value.GetAsPercentageBetween(source, clamp);

    public static Vector3Int AxialToCube(this Vector2Int @this) => new Vector3Int(@this.x, @this.y, -@this.x-@this.y);
    

    #endregion
    #region Vector3

     public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public static float GetDistanceXZ(this Vector3 a, Vector3 b)
    {
        float num1 = a.x - b.x;
        float num2 = a.z - b.z;
        return (float) Math.Sqrt(num1 *  num1 + num2 *  num2);
    }
    
    public static float GetSqrDistanceXZ(this Vector3 a, Vector3 b)
    {
        float num1 = a.x - b.x;
        float num2 = a.z - b.z;
        return num1 *  num1 +  num2 *  num2;
    }
    public static Vector3 GetDirection(this Vector3 startPos, Vector3 endPos) => GetDifference(startPos, endPos).normalized;

    public static Vector3 GetDirectionXZ(this Vector3 startPos, Vector3 endPos) => GetDifferenceXZ(startPos,endPos).normalized;
    
    public static Vector3 GetDifferenceXZ(this Vector3 startPos, Vector3 endPos) => GetDifference(new Vector3(startPos.x,0,startPos.z), new Vector3(endPos.x,0,endPos.z));

    public static float GetMagnitude(this Vector3 startPos, Vector3 endPos) =>
        GetDifference(startPos, endPos).magnitude;


    public static float GetSqrMagnitude(this Vector3 startPos, Vector3 endPos) =>
        GetDifference((Vector2)startPos, (Vector2)endPos).sqrMagnitude;

    public static Vector3 GetDifference(this Vector3 startPos, Vector3 endPos) => (endPos - startPos);

    public static float AsAngle2D(this Vector3 source) => AsAngle(source);

    public static float GetAngleXY(this Vector3 startPos, Vector3 endPos) => ((Vector2)startPos).GetAngle(endPos);
    public static float GetAngleXZ(this Vector3 startPos, Vector3 endPos) => (new Vector2(startPos.x,startPos.z)).GetAngle(new Vector2(endPos.x,endPos.z));

    public static Vector3 SwapYZ(this Vector3 value)
    {
        (value.y, value.z) = (value.z, value.y);
        return value;
    }

    public static Vector3 SnapTo(this Vector3 value, float snap)
    {
        Vector3 retValue = value;
        retValue.x = retValue.x.SnapTo(snap);
        retValue.y = retValue.y.SnapTo(snap);
        retValue.z = retValue.z.SnapTo(snap);
        return retValue;
    }

    public static Vector2Int CubeToAxial(this Vector3Int @this) => new Vector2Int(@this.x, @this.y);

    public static Vector3Int HexagonalRotationLeft(this Vector3Int @this) => HexagonalRotationLeft(@this, Vector3Int.zero);

    public static Vector3Int HexagonalRotationLeft(this Vector3Int @this, Vector3Int pivot)
    {
        var coord = @this - pivot;

        var old = coord;
            
        old.x -= (coord.y - (coord.y & 1)) / 2;

        coord.x = -old.y;
        coord.y = -(-old.x - old.y);

        coord.x += (coord.y - (coord.y & 1)) / 2;

        return coord + pivot;
    }
    
    public static Vector3Int HexagonalRotationRight(this Vector3Int @this) => HexagonalRotationRight(@this, Vector3Int.zero);

    public static Vector3Int HexagonalRotationRight(this Vector3Int @this, Vector3Int pivot)
    {
        var coord = @this - pivot;

        var old = coord;
            
        old.x -= (coord.y - (coord.y & 1)) / 2;

        coord.x = -(-old.x - old.y);
        coord.y = -old.x;

        coord.x += (coord.y - (coord.y & 1)) / 2;

        return coord + pivot;
    }

    #endregion
    #region Vector3Int

    public static Vector3Int GetIndexXYZ(this Vector3Int source, int index)
    {
        Vector3Int value = new Vector3Int
        {
            x = index % source.x,
            y = (index / source.x) % source.y,
            z = index / (source.x * source.y)
        };
        return value;
    }

    public static Vector3Int GetIndexXZY(this Vector3Int source, int index)
    {
        Vector3Int value = new Vector3Int
        {
            x = index % source.x,
            z = (index / source.x) % source.z,
            y = index / (source.x * source.z)
        };
        return value;
    }

    public static Vector3Int GetIndexYZX(this Vector3Int source, int index)
    {
        Vector3Int value = new Vector3Int
        {
            y = index % source.y,
            z = (index / source.y) % source.z,
            x = index / (source.y * source.z)
        };
        return value;
    }

    public static Vector2Int AsVector2Int(this Vector3Int @this) => new Vector2Int(@this.x, @this.y);
    
    public static Vector2Int AsVector2IntXZ(this Vector3Int @this) => new Vector2Int(@this.x, @this.z);

    public static int TotalCount(this Vector3Int source) => source.x * source.y * source.z;

    public static Vector3Int SwapYZ(this Vector3Int value)
    {
        (value.y, value.z) = (value.z, value.y);
        return value;
    }
    #endregion
    
    public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private List<TKey> keyData = new List<TKey>();
        [SerializeField, HideInInspector] private List<TValue> valueData = new List<TValue>();
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();
            
            for (var i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
            {
                this[this.keyData[i]] = this.valueData[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.keyData.Clear();
            this.valueData.Clear();

            foreach (var item in this)
            {
                this.keyData.Add(item.Key);
                this.valueData.Add(item.Value);
            }
        }
    }
}

public static class VectorExtensionMethods {

	public static Vector2 xy(this Vector3 v) {
		return new Vector2(v.x, v.y);
	}

	public static Vector3 WithX(this Vector3 v, float x) {
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y) {
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z) {
		return new Vector3(v.x, v.y, z);
	}

	public static Vector2 WithX(this Vector2 v, float x) {
		return new Vector2(x, v.y);
	}
	
	public static Vector2 WithY(this Vector2 v, float y) {
		return new Vector2(v.x, y);
	}
	
	public static Vector3 WithZ(this Vector2 v, float z) {
		return new Vector3(v.x, v.y, z);
        }
        
    // axisDirection - unit vector in direction of an axis (eg, defines a line that passes through zero)
    // point - the point to find nearest on line for
    public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
    {
        if (!isNormalized) axisDirection.Normalize();
        var d = Vector3.Dot(point, axisDirection);
        return axisDirection * d;
    }

    // lineDirection - unit vector in direction of line
    // pointOnLine - a point on the line (allowing us to define an actual line in space)
    // point - the point to find nearest on line for
    public static Vector3 NearestPointOnLine(
        this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
    {
        if (!isNormalized) lineDirection.Normalize();
        var d = Vector3.Dot(point - pointOnLine, lineDirection);
        return pointOnLine + (lineDirection * d);
    }
}

public static class ExtensionMethods
{

    public static float LinearRemap(this float value,
                                     float valueRangeMin, float valueRangeMax,
                                     float newRangeMin, float newRangeMax)
    {
        return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
    }

    public static int WithRandomSign(this int value, float negativeProbability = 0.5f)
    {
        return Random.value < negativeProbability ? -value : value;
    }

}

