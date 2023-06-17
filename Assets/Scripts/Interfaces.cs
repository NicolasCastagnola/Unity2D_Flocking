public interface IState { void OnEnter(); void OnUpdate(); void OnExit();}
public interface IDestroyable { void Destroy(); }
