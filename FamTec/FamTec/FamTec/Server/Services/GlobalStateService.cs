using FamTec.Shared.Server.DTO.KakaoLog;

namespace FamTec.Server.Services
{
    public class GlobalStateService
    {
        /// <summary>
        /// 전역적으로 사용할 MID 리스트
        /// </summary>
        public List<AddKaKaoSendResult> SendResultList { get; private set; } = new List<AddKaKaoSendResult>();

        /// <summary>
        /// MID 리스트에 값 추가
        /// </summary>
        /// <param name="mid"></param>
        public void AddMID(AddKaKaoSendResult mid)
        {
            List<string> MID = SendResultList.Select(m => m.MID).ToList();
            if (!MID.Contains(mid.MID))
            {
                SendResultList.Add(new AddKaKaoSendResult()
                {
                    PlaceID = mid.PlaceID,
                    VocID = mid.VocID,
                    BuildingID = mid.BuildingID,
                    MID = mid.MID
                });
            }
        }

        /// <summary>
        /// MID 리스트 초기화
        /// </summary>
        public void ClearMID()
        {
            SendResultList.Clear();
        }

        /// <summary>
        /// MID 리스트 반환
        /// </summary>
        /// <returns></returns>
        public List<AddKaKaoSendResult> GetMIDList()
        {
            return SendResultList;
        }


    }
}
