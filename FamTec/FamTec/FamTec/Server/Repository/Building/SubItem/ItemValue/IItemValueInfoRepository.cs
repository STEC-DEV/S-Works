using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Building.SubItem.ItemValue
{
    public interface IItemValueInfoRepository
    {
        /// <summary>
        /// 아이템 KEY에 대한 Value추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<ItemvalueTb?> AddAsync(ItemvalueTb? model);

        /// <summary>
        /// 아이템 Value 리스트 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        ValueTask<List<ItemvalueTb>?> GetAllValueList(int? keyid);

        /// <summary>
        /// 아이템 Value 상세검색 valueid로 검색
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        ValueTask<ItemvalueTb?> GetValueInfo(int? valueid);

        /// <summary>
        /// 아이템 Value 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> UpdateValueInfo(ItemvalueTb? model);

        /// <summary>
        /// 아이템 Value 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteValueInfo(ItemvalueTb? model);

        /// <summary>
        /// 넘어온 keyItemId에 포함되어있는 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<ItemvalueTb>?> ContainsKeyList(List<int> KeyitemId);

        /// <summary>
        /// 넘어온 keyItemId에 포함되어있지 않은 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        ValueTask<List<ItemvalueTb>?> NotContainsKeyList(List<int> KeyitemId);

    }
}
