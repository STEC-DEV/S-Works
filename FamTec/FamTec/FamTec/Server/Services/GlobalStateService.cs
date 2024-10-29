namespace FamTec.Server.Services
{
    public class GlobalStateService
    {
        // 전역적으로 사용할 MID 리스트
        public List<string> MID { get; private set; } = new List<string>();

        // MID 리스트에 값 추가
        public void AddMID(string mid)
        {
            if (!MID.Contains(mid))
            {
                MID.Add(mid);
            }
        }

        // MID 리스트 초기화
        public void ClearMID()
        {
            MID.Clear();
        }

        // MID 리스트 반환
        public List<string> GetMIDList()
        {
            return MID;
        }
    }
}
