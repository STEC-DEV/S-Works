﻿namespace FamTec.Shared.Server.DTO.Facility.Group
{
    public class UpdateKeyDTO
    {
        public int? ID { get; set; }
        public string? Itemkey { get; set; }
        public string? Unit { get; set; }
        public List<GroupValueListDTO>? ValueList { get; set; } = new List<GroupValueListDTO>();
    }
}
