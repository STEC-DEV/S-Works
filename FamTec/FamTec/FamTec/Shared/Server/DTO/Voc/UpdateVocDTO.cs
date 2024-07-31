namespace FamTec.Shared.Server.DTO.Voc
{
    public class UpdateVocDTO
    {
        /// <summary>
        /// VOC ID
        /// </summary>
        public int VocID { get; set; }

        /// <summary>
        /// VOC 유형 (미화, 보안, 미분류..)
        /// </summary>
        public int Type { get; set; }
    }
}
