namespace CCGP.Shared
{
    public interface IState
    {
        void OnInitialize();
        void OnEnter();
        void OnUpdate();
        void OnExit();
        void OnClear();
        void OnNextState(IState next);
    }
}