using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Drawer
{
    public class PlacePermDTO
    {
        /// <summary>
        /// 사업장 ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 기계설비 권한
        /// </summary>
        public bool? PermMachine { get; set; }

        /// <summary>
        /// 전기설비 권한
        /// </summary>
        public bool? PermElec { get; set; }

        /// <summary>
        /// 승강설비 권한
        /// </summary>
        public bool? PermLift { get; set; }

        /// <summary>
        /// 소방설비 권한
        /// </summary>
        public bool? PermFire { get; set; }

        /// <summary>
        /// 건축설비 권한
        /// </summary>
        public bool? PermConstruct { get; set; }

        /// <summary>
        /// 통신설비 권한
        /// </summary>
        public bool? PermNetwork { get; set; }

        /// <summary>
        /// 미화설비 권한
        /// </summary>
        public bool? PermBeauty { get; set; }

        /// <summary>
        /// 보안설비 권한
        /// </summary>
        public bool? PermSecurity { get; set; }

        /// <summary>
        /// 자재 권한
        /// </summary>
        public bool? PermMaterial { get; set; }

        /// <summary>
        /// 에너지 권한
        /// </summary>
        public bool? PermEnergy { get; set; }

        /// <summary>
        /// 민원 권한
        /// </summary>
        public bool? PermVoc { get; set; }

    }
}
