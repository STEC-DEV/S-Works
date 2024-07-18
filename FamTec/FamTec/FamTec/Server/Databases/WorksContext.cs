using System;
using System.Collections.Generic;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Databases;

public partial class WorksContext : DbContext
{
    public WorksContext()
    {
    }

    public WorksContext(DbContextOptions<WorksContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminPlaceTb> AdminPlaceTbs { get; set; }

    public virtual DbSet<AdminTb> AdminTbs { get; set; }

    public virtual DbSet<AlarmTb> AlarmTbs { get; set; }

    public virtual DbSet<BuildingItemGroupTb> BuildingItemGroupTbs { get; set; }

    public virtual DbSet<BuildingItemKeyTb> BuildingItemKeyTbs { get; set; }

    public virtual DbSet<BuildingItemValueTb> BuildingItemValueTbs { get; set; }

    public virtual DbSet<BuildingTb> BuildingTbs { get; set; }

    public virtual DbSet<CommentTb> CommentTbs { get; set; }

    public virtual DbSet<DepartmentsTb> DepartmentsTbs { get; set; }

    public virtual DbSet<EnergyMonthUsageTb> EnergyMonthUsageTbs { get; set; }

    public virtual DbSet<EnergyUsageTb> EnergyUsageTbs { get; set; }

    public virtual DbSet<FacilityItemGroupTb> FacilityItemGroupTbs { get; set; }

    public virtual DbSet<FacilityItemKeyTb> FacilityItemKeyTbs { get; set; }

    public virtual DbSet<FacilityItemValueTb> FacilityItemValueTbs { get; set; }

    public virtual DbSet<FacilityTb> FacilityTbs { get; set; }

    public virtual DbSet<FloorTb> FloorTbs { get; set; }

    public virtual DbSet<KakaoLogTb> KakaoLogTbs { get; set; }

    public virtual DbSet<MaintenenceHistoryTb> MaintenenceHistoryTbs { get; set; }

    public virtual DbSet<MaterialTb> MaterialTbs { get; set; }

    public virtual DbSet<MeterItemTb> MeterItemTbs { get; set; }

    public virtual DbSet<PlaceTb> PlaceTbs { get; set; }

    public virtual DbSet<RoomTb> RoomTbs { get; set; }

    public virtual DbSet<StoreTb> StoreTbs { get; set; }

    public virtual DbSet<UnitTb> UnitTbs { get; set; }

    public virtual DbSet<UsedMaterialTb> UsedMaterialTbs { get; set; }

    public virtual DbSet<UsersTb> UsersTbs { get; set; }

    public virtual DbSet<VocTb> VocTbs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=123.2.156.122,3306;database=Works;user id=root;password=stecdev1234!", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.7-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AdminPlaceTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("admin_place_tb", tb => tb.HasComment("관리자 사업장테이블"));

            entity.Property(e => e.Id).HasComment("관리자 사업장 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");

            entity.HasOne(d => d.AdminTb).WithMany(p => p.AdminPlaceTbs).HasConstraintName("fk_admin_place_tb_admin_tb1");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.AdminPlaceTbs).HasConstraintName("fk_admin_place_tb_place_tb1");
        });

        modelBuilder.Entity<AdminTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("admin_tb", tb => tb.HasComment("관리자 테이블"));

            entity.Property(e => e.Id).HasComment("관리자 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.DepartmentTbId).HasComment("부서 인덱스\\n");
            entity.Property(e => e.Type).HasComment("계정유형");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.UserTbId).HasComment("사용자 인덱스");

            entity.HasOne(d => d.DepartmentTb).WithMany(p => p.AdminTbs).HasConstraintName("fk_admin_tb_departments_tb1");

            entity.HasOne(d => d.UserTb).WithMany(p => p.AdminTbs).HasConstraintName("fk_admin_tb_users_tb");
        });

        modelBuilder.Entity<AlarmTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alarm_tb", tb => tb.HasComment("실시간_알람"));

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.UsersTb).WithMany(p => p.AlarmTbs).HasConstraintName("fk_alarm_tb_users_tb1");

            entity.HasOne(d => d.VocTb).WithMany(p => p.AlarmTbs).HasConstraintName("fk_alarm_tb_voc_tb1");
        });

        modelBuilder.Entity<BuildingItemGroupTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("building_item_group_tb", tb => tb.HasComment("건물>그룹추가항목"));

            entity.Property(e => e.Id).HasComment("그룹 아이디");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("그룹명");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.BuildingItemGroupTbs).HasConstraintName("fk_building_item_group_building_tb1");
        });

        modelBuilder.Entity<BuildingItemKeyTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("building_item_key_tb", tb => tb.HasComment("건물>그룹항목>키"));

            entity.Property(e => e.Id).HasComment("요소명 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("요소명");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingGroupTb).WithMany(p => p.BuildingItemKeyTbs).HasConstraintName("fk_building_item_key_building_item_group1");
        });

        modelBuilder.Entity<BuildingItemValueTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("building_item_value_tb", tb => tb.HasComment("건물>그룹항목>키>값"));

            entity.Property(e => e.Id).HasComment("값 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.ItemValue).HasComment("값");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingKeyTb).WithMany(p => p.BuildingItemValueTbs).HasConstraintName("fk_building_item_value_building_item_key1");
        });

        modelBuilder.Entity<BuildingTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("building_tb", tb => tb.HasComment("건물"));

            entity.Property(e => e.Id).HasComment("건물 인덱스");
            entity.Property(e => e.Address).HasComment("주소");
            entity.Property(e => e.BasementFloorNum).HasComment("지하");
            entity.Property(e => e.BasementHeight).HasComment("지하");
            entity.Property(e => e.Boiler).HasComment("보일러");
            entity.Property(e => e.BuildingArea).HasComment("건물면적");
            entity.Property(e => e.BuildingCd).HasComment("건물코드");
            entity.Property(e => e.BuildingHeight).HasComment("건물높이");
            entity.Property(e => e.BuildingStruct).HasComment("건물구조");
            entity.Property(e => e.CargoLiftNum).HasComment("화물용");
            entity.Property(e => e.CompletionDt).HasComment("준공년월");
            entity.Property(e => e.ConstComp).HasComment("시공업체");
            entity.Property(e => e.CoolCapacity).HasComment("냉방용량");
            entity.Property(e => e.CoolHeatCapacity).HasComment("냉난방용량");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.ElecCapacity).HasComment("전기용량");
            entity.Property(e => e.ElevWaterCapacity).HasComment("고가수조");
            entity.Property(e => e.FaucetCapacity).HasComment("수전용량");
            entity.Property(e => e.FireRating).HasComment("소방등급");
            entity.Property(e => e.FloorNum).HasComment("건물층수");
            entity.Property(e => e.GasCapacity).HasComment("가스용량");
            entity.Property(e => e.GenerationCapacity).HasComment("발전용량");
            entity.Property(e => e.GrossFloorArea).HasComment("연면적");
            entity.Property(e => e.GroundArea).HasComment("지상면적");
            entity.Property(e => e.GroundFloorNum).HasComment("지상");
            entity.Property(e => e.GroundHeight).HasComment("지상");
            entity.Property(e => e.HeatCapacity).HasComment("난방용량");
            entity.Property(e => e.Image).HasComment("이미지");
            entity.Property(e => e.InnerParkingNum).HasComment("옥내");
            entity.Property(e => e.LandArea).HasComment("대지면적");
            entity.Property(e => e.LandscapeArea).HasComment("조경면적");
            entity.Property(e => e.LiftNum).HasComment("승강기대수");
            entity.Property(e => e.MenToiletNum).HasComment("남자");
            entity.Property(e => e.Name).HasComment("건물명");
            entity.Property(e => e.OuterParkingNum).HasComment("옥외");
            entity.Property(e => e.ParkingNum).HasComment("주차대수");
            entity.Property(e => e.PeopleLiftNum).HasComment("인승용");
            entity.Property(e => e.PlaceTbId).HasComment("사업장 인덱스");
            entity.Property(e => e.RoofStruct).HasComment("지붕구조");
            entity.Property(e => e.RooftopArea).HasComment("옥상");
            entity.Property(e => e.SepticTankCapacity).HasComment("정화조용량");
            entity.Property(e => e.Tel).HasComment("전화번호");
            entity.Property(e => e.ToiletNum).HasComment("화잘실수");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.Usage).HasComment("건물용도");
            entity.Property(e => e.WaterCapacity).HasComment("급수용량");
            entity.Property(e => e.WaterDispenser).HasComment("냉온수기");
            entity.Property(e => e.WaterTank).HasComment("저수조");
            entity.Property(e => e.WomenToiletNum).HasComment("여자");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.BuildingTbs).HasConstraintName("fk_building_tb_place_tb1");
        });

        modelBuilder.Entity<CommentTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comment_tb", tb => tb.HasComment("민원 답변"));

            entity.Property(e => e.Content).HasComment("댓글내용");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Status).HasComment("처리상태");

            entity.HasOne(d => d.VocTb).WithMany(p => p.CommentTbs).HasConstraintName("fk_comment_tb_voc_tb1");
        });

        modelBuilder.Entity<DepartmentsTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments_tb", tb => tb.HasComment("부서"));

            entity.Property(e => e.Id).HasComment("부서 인덱스");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Name).HasComment("부서명");
        });

        modelBuilder.Entity<EnergyMonthUsageTb>(entity =>
        {
            entity.HasKey(e => e.MonthUsageId).HasName("PRIMARY");

            entity.ToTable("energy_month_usage_tb", tb => tb.HasComment("에너지 월별 사용량"));

            entity.Property(e => e.Apr).HasComment("4월");
            entity.Property(e => e.Aug).HasComment("8월");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Dec).HasComment("12월");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Feb).HasComment("2월");
            entity.Property(e => e.Jan).HasComment("1월");
            entity.Property(e => e.Jul).HasComment("7월");
            entity.Property(e => e.Jun).HasComment("6월");
            entity.Property(e => e.Mar).HasComment("3월");
            entity.Property(e => e.May).HasComment("5월");
            entity.Property(e => e.MeterItemId).HasComment("검침기 인덱스");
            entity.Property(e => e.Nov).HasComment("11월");
            entity.Property(e => e.Oct).HasComment("10월");
            entity.Property(e => e.Sep).HasComment("9월");

            entity.HasOne(d => d.MeterItem).WithMany(p => p.EnergyMonthUsageTbs).HasConstraintName("fk_energy_month_usage_tb_meter_item_tb1");
        });

        modelBuilder.Entity<EnergyUsageTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("energy_usage_tb", tb => tb.HasComment("에너지 검침 기록 - 일별"));

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.MeterDt).HasComment("검침일자");
            entity.Property(e => e.UseAmount).HasComment("사용량");

            entity.HasOne(d => d.MeterItem).WithMany(p => p.EnergyUsageTbs).HasConstraintName("fk_energy_usage_tb_meter_item_tb1");
        });

        modelBuilder.Entity<FacilityItemGroupTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("facility_item_group_tb", tb => tb.HasComment("설비 > 그룹"));

            entity.Property(e => e.Id).HasComment("그룹 아이디");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("그룹명");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FacilityTb).WithMany(p => p.FacilityItemGroupTbs).HasConstraintName("fk_facility_item_group_facility_tb1");
        });

        modelBuilder.Entity<FacilityItemKeyTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("facility_item_key_tb", tb => tb.HasComment("설비 > 그룹 > 키"));

            entity.Property(e => e.Id).HasComment("요소명 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("요소명");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FacilityItemGroupTb).WithMany(p => p.FacilityItemKeyTbs).HasConstraintName("fk_facility_item_key_facility_item_group1");
        });

        modelBuilder.Entity<FacilityItemValueTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("facility_item_value_tb", tb => tb.HasComment("설비 > 그룹 > 키 > 값"));

            entity.Property(e => e.Id).HasComment("값 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.ItemValue).HasComment("값");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FacilityItemKeyTb).WithMany(p => p.FacilityItemValueTbs).HasConstraintName("fk_facility_item_value_facility_item_key1");
        });

        modelBuilder.Entity<FacilityTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("facility_tb", tb => tb.HasComment("설비"));

            entity.Property(e => e.Id).HasComment("설비 인덱스");
            entity.Property(e => e.Category).HasComment("카테고리");
            entity.Property(e => e.ChangeDt).HasComment("교체년월");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.EquipDt).HasComment("설치년월");
            entity.Property(e => e.Image).HasComment("이미지");
            entity.Property(e => e.Lifespan).HasComment("내용연수");
            entity.Property(e => e.Name).HasComment("설비명칭");
            entity.Property(e => e.Num).HasComment("수량");
            entity.Property(e => e.StandardCapacity).HasComment("규격용량");
            entity.Property(e => e.Type).HasComment("형식");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.FacilityTbs).HasConstraintName("fk_facility_tb_room_tb1");
        });

        modelBuilder.Entity<FloorTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("floor_tb", tb => tb.HasComment("층"));

            entity.Property(e => e.Id).HasComment("층 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("층 이름");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.FloorTbs).HasConstraintName("fk_floor_tb_building_tb1");
        });

        modelBuilder.Entity<KakaoLogTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<MaintenenceHistoryTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("maintenence_history_tb", tb => tb.HasComment("유지보수 이력"));

            entity.Property(e => e.Id).HasComment("이력 인덱스");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Name).HasComment("이력명");
            entity.Property(e => e.Num).HasComment("수량");
            entity.Property(e => e.TotalPrice).HasComment("소요비용");
            entity.Property(e => e.Type).HasComment("작업구분");
            entity.Property(e => e.UnitPrice).HasComment("단가");
            entity.Property(e => e.Worker).HasComment("작업자");

            entity.HasOne(d => d.FacilityTb).WithMany(p => p.MaintenenceHistoryTbs).HasConstraintName("fk_maintenence_history_tb_facility_tb1");
        });

        modelBuilder.Entity<MaterialTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("material_tb", tb => tb.HasComment("자재"));

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DefaultLocation).HasComment("공간인덱스");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Image).HasComment("이미지");
            entity.Property(e => e.ManufacturingComp).HasComment("제조사");
            entity.Property(e => e.Name).HasComment("자재명");
            entity.Property(e => e.SafeNum).HasComment("안전재고수량");
            entity.Property(e => e.Standard).HasComment("규격");
            entity.Property(e => e.Unit).HasComment("단위");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.MaterialTbs).HasConstraintName("fk_material_tb_place_tb1");
        });

        modelBuilder.Entity<MeterItemTb>(entity =>
        {
            entity.HasKey(e => e.MeterItemId).HasName("PRIMARY");

            entity.ToTable("meter_item_tb", tb => tb.HasComment("검침기 + 항목"));

            entity.Property(e => e.AccumUsage).HasComment("누적사용량");
            entity.Property(e => e.Category).HasComment("전기, 기계 ..");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.MeterItem).HasComment("검침항목");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.MeterItemTbs).HasConstraintName("fk_meter_item_tb_building_tb1");
        });

        modelBuilder.Entity<PlaceTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("사업장 인덱스");
            entity.Property(e => e.Address).HasComment("주소");
            entity.Property(e => e.CancelDt).HasComment("해약일자");
            entity.Property(e => e.ContractDt).HasComment("계약일자");
            entity.Property(e => e.ContractNum).HasComment("계약번호");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("사업장명");
            entity.Property(e => e.Note).HasComment("비고");
            entity.Property(e => e.PermBeauty)
                .HasDefaultValueSql("'0'")
                .HasComment("미화 권한");
            entity.Property(e => e.PermConstruct)
                .HasDefaultValueSql("'0'")
                .HasComment("건축관리 권한");
            entity.Property(e => e.PermElec)
                .HasDefaultValueSql("'0'")
                .HasComment("전기관리 권한");
            entity.Property(e => e.PermEnergy)
                .HasDefaultValueSql("'0'")
                .HasComment("에너지관리 권한");
            entity.Property(e => e.PermFire)
                .HasDefaultValueSql("'0'")
                .HasComment("소방관리 권한");
            entity.Property(e => e.PermLift)
                .HasDefaultValueSql("'0'")
                .HasComment("승강관리 권한");
            entity.Property(e => e.PermMachine)
                .HasDefaultValueSql("'0'")
                .HasComment("기게정보권한");
            entity.Property(e => e.PermMaterial)
                .HasDefaultValueSql("'0'")
                .HasComment("자재관리 권한");
            entity.Property(e => e.PermNetwork)
                .HasDefaultValueSql("'0'")
                .HasComment("통신관리 권한");
            entity.Property(e => e.PermSecurity)
                .HasDefaultValueSql("'0'")
                .HasComment("보안 권한");
            entity.Property(e => e.PermVoc)
                .HasDefaultValueSql("'0'")
                .HasComment("민원관리 권한");
            entity.Property(e => e.PlaceCd).HasComment("사업장 코드");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("계약상태");
            entity.Property(e => e.Tel).HasComment("전화번호");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
        });

        modelBuilder.Entity<RoomTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("공간 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("공간명");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FloorTb).WithMany(p => p.RoomTbs).HasConstraintName("fk_room_tb_floor_tb1");
        });

        modelBuilder.Entity<StoreTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Inout).HasComment("입출고 구분");
            entity.Property(e => e.InoutDate).HasComment("입출고 날짜");
            entity.Property(e => e.Num).HasComment("수량");
            entity.Property(e => e.TotalPrice).HasComment("입출고 가격");
            entity.Property(e => e.UnitPrice).HasComment("단가");

            entity.HasOne(d => d.MaterialTb).WithMany(p => p.StoreTbs).HasConstraintName("fk_store_tb_material_tb1");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.StoreTbs).HasConstraintName("fk_store_tb_room_tb1");
        });

        modelBuilder.Entity<UnitTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("단위 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.PlaceTbId).HasComment("사업장 인덱스");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.UnitTbs).HasConstraintName("fk_unit_tb_place_tb1");
        });

        modelBuilder.Entity<UsedMaterialTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("사용자재 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.MaintenenceHistoryTb).WithMany(p => p.UsedMaterialTbs).HasConstraintName("fk_used_material_maintenence_history_tb1");

            entity.HasOne(d => d.MaterialTb).WithMany(p => p.UsedMaterialTbs).HasConstraintName("fk_used_material_material_tb1");
        });

        modelBuilder.Entity<UsersTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("사용자 인덱스");
            entity.Property(e => e.AdminYn)
                .HasDefaultValueSql("'0'")
                .HasComment("관리자 여부");
            entity.Property(e => e.AlarmYn)
                .HasDefaultValueSql("'0'")
                .HasComment("알람여부");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일자");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일자");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Email).HasComment("이메일");
            entity.Property(e => e.Image).HasComment("이미지");
            entity.Property(e => e.Job).HasComment("직책\\r\\n");
            entity.Property(e => e.Name).HasComment("이름");
            entity.Property(e => e.Password).HasComment("비밀번호");
            entity.Property(e => e.PermBasic)
                .HasDefaultValueSql("'0'")
                .HasComment("기본정보 권한");
            entity.Property(e => e.PermBeauty)
                .HasDefaultValueSql("'0'")
                .HasComment("미화 권한");
            entity.Property(e => e.PermConstruct)
                .HasDefaultValueSql("'0'")
                .HasComment("건축관리 권한");
            entity.Property(e => e.PermElec)
                .HasDefaultValueSql("'0'")
                .HasComment("전기관리 권한");
            entity.Property(e => e.PermEnergy)
                .HasDefaultValueSql("'0'")
                .HasComment("에너지관리 권한");
            entity.Property(e => e.PermFire)
                .HasDefaultValueSql("'0'")
                .HasComment("소방관리 권한");
            entity.Property(e => e.PermLift)
                .HasDefaultValueSql("'0'")
                .HasComment("승강관리 권한");
            entity.Property(e => e.PermMachine)
                .HasDefaultValueSql("'0'")
                .HasComment("기계관리 권한");
            entity.Property(e => e.PermMaterial)
                .HasDefaultValueSql("'0'")
                .HasComment("자재관리 권한");
            entity.Property(e => e.PermNetwork)
                .HasDefaultValueSql("'0'")
                .HasComment("통신연동 권한");
            entity.Property(e => e.PermSecurity)
                .HasDefaultValueSql("'0'")
                .HasComment("보안 권한");
            entity.Property(e => e.PermUser)
                .HasDefaultValueSql("'0'")
                .HasComment("사용자 관리 권한");
            entity.Property(e => e.PermVoc)
                .HasDefaultValueSql("'0'")
                .HasComment("민원관리 권한");
            entity.Property(e => e.Phone).HasComment("전화번호");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("재직여부");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.UserId).HasComment("사용자 아아디");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.UsersTbs).HasConstraintName("fk_users_tb_place_tb1");
        });

        modelBuilder.Entity<VocTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.BuildingTbId).HasComment("건물 인덱스");
            entity.Property(e => e.Content).HasComment("민원내용");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Image1).HasComment("이미지");
            entity.Property(e => e.Image2).HasComment("이미지");
            entity.Property(e => e.Image3).HasComment("이미지");
            entity.Property(e => e.Phone).HasComment("전화번호");
            entity.Property(e => e.ReplyYn)
                .HasDefaultValueSql("'0'")
                .HasComment("답변회신여부");
            entity.Property(e => e.Status).HasComment("민원처리상태");
            entity.Property(e => e.Title).HasComment("민원제목");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
