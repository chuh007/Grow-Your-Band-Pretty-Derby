namespace Code.Core.Bus.GameEvents
{
    /// <summary>
    ///  화면 회전할때 사용하는 버스
    ///  Isverticalscreen이 True면 세로화면 false면 가로화면 회전할때만 사용하셈
    /// </summary>
    public struct ScreenRotationEvent : IEvent
    {
        public bool Isverticalscreen;

        public ScreenRotationEvent(bool isverticalscreen)
        {
            Isverticalscreen = isverticalscreen;
        }
    }
}