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

    public virtual DbSet<BlacklistTb> BlacklistTbs { get; set; }

    public virtual DbSet<BuildingItemGroupTb> BuildingItemGroupTbs { get; set; }

    public virtual DbSet<BuildingItemKeyTb> BuildingItemKeyTbs { get; set; }

    public virtual DbSet<BuildingItemValueTb> BuildingItemValueTbs { get; set; }

    public virtual DbSet<BuildingTb> BuildingTbs { get; set; }

    public virtual DbSet<CommentTb> CommentTbs { get; set; }

    public virtual DbSet<ContractTypeTb> ContractTypeTbs { get; set; }

    public virtual DbSet<DepartmentsTb> DepartmentsTbs { get; set; }

    public virtual DbSet<ElecEnergyAmountTb> ElecEnergyAmountTbs { get; set; }

    public virtual DbSet<EnergyDayUsageTb> EnergyDayUsageTbs { get; set; }

    public virtual DbSet<EnergyMonthUsageTb> EnergyMonthUsageTbs { get; set; }

    public virtual DbSet<FacilityItemGroupTb> FacilityItemGroupTbs { get; set; }

    public virtual DbSet<FacilityItemKeyTb> FacilityItemKeyTbs { get; set; }

    public virtual DbSet<FacilityItemValueTb> FacilityItemValueTbs { get; set; }

    public virtual DbSet<FacilityTb> FacilityTbs { get; set; }

    public virtual DbSet<FloorTb> FloorTbs { get; set; }

    public virtual DbSet<InventoryTb> InventoryTbs { get; set; }

    public virtual DbSet<KakaoLogTb> KakaoLogTbs { get; set; }

    public virtual DbSet<MaintenenceHistoryTb> MaintenenceHistoryTbs { get; set; }

    public virtual DbSet<MaterialTb> MaterialTbs { get; set; }

    public virtual DbSet<MeterItemTb> MeterItemTbs { get; set; }

    public virtual DbSet<PlaceTb> PlaceTbs { get; set; }

    public virtual DbSet<RoomTb> RoomTbs { get; set; }

    public virtual DbSet<StoreTb> StoreTbs { get; set; }

    public virtual DbSet<UnitTb> UnitTbs { get; set; }

    public virtual DbSet<UseMaintenenceMaterialTb> UseMaintenenceMaterialTbs { get; set; }

    public virtual DbSet<UsersTb> UsersTbs { get; set; }

    public virtual DbSet<VocTb> VocTbs { get; set; }

    public virtual DbSet<MaterialInventory> MaterialInven { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

        string? ConnStr = configuration.GetConnectionString("DefaultConnection");
        if (!String.IsNullOrWhiteSpace(ConnStr))
        {
            optionsBuilder.UseMySql(ConnStr, ServerVersion.Parse("10.11.7-mariadb"),
                mySqlOption =>
                {
                    mySqlOption.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null); // 자동 재시도 설정 (최대 3회, 5초 대기)
                    mySqlOption.CommandTimeout(60);
                    mySqlOption.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // 복잡한 쿼리의 성능 향상을 위한 쿼리 분할 사용
                });
        }
        else
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is null or empty.");
        }

        //optionsBuilder.UseMySql("server=123.2.156.122,3306;database=Works;user id=root;password=stecdev1234!", ServerVersion.Parse("10.11.7-mariadb"),
        //    mySqlOption =>
        //    {
        //        mySqlOption.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null); // 자동 재시도 설정 (최대 3회, 5초 대기)
        //        mySqlOption.CommandTimeout(60);
        //        mySqlOption.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // 복잡한 쿼리의 성능 향상을 위한 쿼리 분할 사용
        //    });
    }

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

            entity.HasOne(d => d.AdminTb).WithMany(p => p.AdminPlaceTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_place_tb_admin_tb1");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.AdminPlaceTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_place_tb_place_tb1");
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
            entity.Property(e => e.DepartmentTbId).HasComment("부서 인덱스\\\\n");
            entity.Property(e => e.Type).HasComment("계정유형");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.UserTbId).HasComment("사용자 인덱스");

            entity.HasOne(d => d.DepartmentTb).WithMany(p => p.AdminTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_tb_departments_tb1");

            entity.HasOne(d => d.UserTb).WithMany(p => p.AdminTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_tb_users_tb");
        });

        modelBuilder.Entity<AlarmTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alarm_tb", tb => tb.HasComment("실시간_알람"));

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Type).HasComment("0: 접수 / 1: 변경 / ....");

            entity.HasOne(d => d.UsersTb).WithMany(p => p.AlarmTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_alarm_tb_users_tb1");

            entity.HasOne(d => d.VocTb).WithMany(p => p.AlarmTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_alarm_tb_voc_tb1");
        });

        modelBuilder.Entity<BlacklistTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
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

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.BuildingItemGroupTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_building_item_group_building_tb1");
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
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingGroupTb).WithMany(p => p.BuildingItemKeyTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_building_item_key_building_item_group1");
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
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingKeyTb).WithMany(p => p.BuildingItemValueTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_building_item_value_building_item_key1");
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

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.BuildingTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_building_tb_place_tb1");
        });

        modelBuilder.Entity<CommentTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comment_tb", tb => tb.HasComment("민원 답변"));

            entity.Property(e => e.Content).HasComment("댓글내용");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Status).HasComment("처리상태");

            entity.HasOne(d => d.UserTb).WithMany(p => p.CommentTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUsertbId_202407311313");

            entity.HasOne(d => d.VocTb).WithMany(p => p.CommentTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_comment_tb_voc_tb1");
        });

        modelBuilder.Entity<ContractTypeTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.Name).HasComment("계약종류");
            entity.Property(e => e.PlaceTbId).HasComment("사업장 외래키");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.ContractTypeTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PLACE_TB_ID_202409021046");
        });

        modelBuilder.Entity<DepartmentsTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments_tb", tb => tb.HasComment("부서"));

            entity.Property(e => e.Id).HasComment("부서 인덱스");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.ManagementYn).HasComment("관리부서YN");
            entity.Property(e => e.Name).HasComment("부서명");
        });

        modelBuilder.Entity<ElecEnergyAmountTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("elec_energy_amount_tb", tb => tb.HasComment("전기 월청구 요금"));

            entity.Property(e => e.ChargePrice)
                .HasDefaultValueSql("'0'")
                .HasComment("월 청구요금");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Month).HasComment("월");
            entity.Property(e => e.Year).HasComment("년");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.ElecEnergyAmountTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_place_tb_id_20241014");
        });

        modelBuilder.Entity<EnergyDayUsageTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("energy_day_usage_tb", tb => tb.HasComment("에너지 검침 기록 - 일별"));

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Days).HasComment("일");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.MeterDt).HasComment("검침일자");
            entity.Property(e => e.Month).HasComment("월");
            entity.Property(e => e.TotalAmount).HasComment("사용량");
            entity.Property(e => e.Year).HasComment("년도");

            entity.HasOne(d => d.MeterItem).WithMany(p => p.EnergyDayUsageTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_energy_usage_tb_meter_item_tb1");
        });

        modelBuilder.Entity<EnergyMonthUsageTb>(entity =>
        {
            entity.HasKey(e => e.MonthUsageId).HasName("PRIMARY");

            entity.ToTable("energy_month_usage_tb", tb => tb.HasComment("에너지 월별 사용량"));

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.MeterItemId).HasComment("검침기 인덱스");
            entity.Property(e => e.Month).HasComment("월");
            entity.Property(e => e.TotalUsage)
                .HasDefaultValueSql("'0'")
                .HasComment("월 총사용량");
            entity.Property(e => e.UnitPrice)
                .HasDefaultValueSql("'0'")
                .HasComment("단가금액");
            entity.Property(e => e.Year).HasComment("년도");

            entity.HasOne(d => d.MeterItem).WithMany(p => p.EnergyMonthUsageTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_energy_month_usage_tb_meter_item_tb1");
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

            entity.HasOne(d => d.FacilityTb).WithMany(p => p.FacilityItemGroupTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_facility_item_group_facility_tb1");
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
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FacilityItemGroupTb).WithMany(p => p.FacilityItemKeyTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_facility_item_key_facility_item_group1");
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
            entity.Property(e => e.DelYn).HasComment("삭제여부");
            entity.Property(e => e.ItemValue).HasComment("값");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FacilityItemKeyTb).WithMany(p => p.FacilityItemValueTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_facility_item_value_facility_item_key1");
        });

        modelBuilder.Entity<FacilityTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("facility_tb", tb => tb.HasComment("설비"));

            entity.Property(e => e.Id).HasComment("설비 인덱스");
            entity.Property(e => e.Category).HasComment("카테고리 (설비유형)");
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
            entity.Property(e => e.Lifespan).HasComment("내용연수");
            entity.Property(e => e.Name).HasComment("설비명칭");
            entity.Property(e => e.Num).HasComment("수량");
            entity.Property(e => e.StandardCapacity).HasComment("규격용량");
            entity.Property(e => e.Type).HasComment("형식");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.FacilityTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_facility_tb_room_tb1");
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

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.FloorTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_floor_tb_building_tb1");
        });

        modelBuilder.Entity<InventoryTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.MaterialTb).WithMany(p => p.InventoryTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_invenory_tb_material_tb1");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.InventoryTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_invenory_tb_place_tb1");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.InventoryTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_invenory_tb_room_tb1");
        });

        modelBuilder.Entity<KakaoLogTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.BuildingTbId).HasComment("건물ID");
            entity.Property(e => e.Code).HasComment("전송결과 코드");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Message).HasComment("전송결과 메시지");
            entity.Property(e => e.PlaceTbId).HasComment("사업장ID");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.KakaoLogTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BUILDING_TB_ID_202408080816");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.KakaoLogTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PLACE_TB_ID_202408080816");

            entity.HasOne(d => d.VocTb).WithMany(p => p.KakaoLogTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VOC_TB_ID_202408080816");
        });

        modelBuilder.Entity<MaintenenceHistoryTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("maintenence_history_tb", tb => tb.HasComment("유지보수 이력"));

            entity.Property(e => e.Id).HasComment("이력 인덱스");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Name).HasComment("이력명");
            entity.Property(e => e.Note).HasComment("유지보수 취소사유 설명");
            entity.Property(e => e.TotalPrice).HasComment("소요비용");
            entity.Property(e => e.Type).HasComment("작업구분");
            entity.Property(e => e.Workdt).HasComment("작업일자");
            entity.Property(e => e.Worker).HasComment("작업자");

            entity.HasOne(d => d.FacilityTb).WithMany(p => p.MaintenenceHistoryTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maintenence_history_tb_facility_tb1");
        });

        modelBuilder.Entity<MaterialTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("material_tb", tb => tb.HasComment("자재"));

            entity.Property(e => e.Code).HasComment("품목코드");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.ManufacturingComp).HasComment("제조사");
            entity.Property(e => e.Name).HasComment("자재명");
            entity.Property(e => e.RoomTbId).HasComment("기본위치");
            entity.Property(e => e.SafeNum).HasComment("안전재고수량");
            entity.Property(e => e.Standard).HasComment("규격");
            entity.Property(e => e.Unit).HasComment("단위");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.MaterialTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_material_tb_place_tb1");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.MaterialTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_room_Tb_id");
        });

        modelBuilder.Entity<MeterItemTb>(entity =>
        {
            entity.HasKey(e => e.MeterItemId).HasName("PRIMARY");

            entity.ToTable("meter_item_tb", tb => tb.HasComment("검침기 + 항목"));

            entity.Property(e => e.Category).HasComment("전기, 기계 ..");
            entity.Property(e => e.ContractTbId).HasComment("계약종");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Name).HasComment("계량기이름");

            entity.HasOne(d => d.ContractTb).WithMany(p => p.MeterItemTbs).HasConstraintName("fk_contract_tb_id");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.MeterItemTbs).HasConstraintName("fk_place_tb_id");
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
            entity.Property(e => e.DepartmentTbId).HasComment("관리부서 인덱스");
            entity.Property(e => e.Name).HasComment("사업장명");
            entity.Property(e => e.PermBeauty).HasComment("미화 권한");
            entity.Property(e => e.PermConstruct).HasComment("건축관리 권한");
            entity.Property(e => e.PermElec).HasComment("전기관리 권한");
            entity.Property(e => e.PermEnergy).HasComment("에너지관리 권한");
            entity.Property(e => e.PermFire).HasComment("소방관리 권한");
            entity.Property(e => e.PermLift).HasComment("승강관리 권한");
            entity.Property(e => e.PermMachine).HasComment("기게정보권한");
            entity.Property(e => e.PermMaterial).HasComment("자재관리 권한");
            entity.Property(e => e.PermNetwork).HasComment("통신관리 권한");
            entity.Property(e => e.PermSecurity).HasComment("보안 권한");
            entity.Property(e => e.PermVoc).HasComment("민원관리 권한");
            entity.Property(e => e.Status).HasComment("계약상태");
            entity.Property(e => e.Tel).HasComment("전화번호");
            entity.Property(e => e.UpdateDt).HasComment("수정일자");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.DepartmentTb).WithMany(p => p.PlaceTbs).HasConstraintName("FK_departmenttb_20240806");
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

            entity.HasOne(d => d.FloorTb).WithMany(p => p.RoomTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_room_tb_floor_tb1");
        });

        modelBuilder.Entity<StoreTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.CurrentNum).HasComment("현재재고수량");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Inout).HasComment("입출고 구분");
            entity.Property(e => e.InoutDate).HasComment("입출고 날짜");
            entity.Property(e => e.MaintenenceHistoryTbId).HasComment("유지보수이력ID");
            entity.Property(e => e.MaterialTbId).HasComment("품목ID");
            entity.Property(e => e.Note).HasComment("비고");
            entity.Property(e => e.Note2).HasComment("유지보수취소_시스템설명");
            entity.Property(e => e.Num).HasComment("수량");
            entity.Property(e => e.PlaceTbId).HasComment("사업장ID");
            entity.Property(e => e.RoomTbId).HasComment("공간ID");
            entity.Property(e => e.TotalPrice).HasComment("입출고 가격");
            entity.Property(e => e.UnitPrice).HasComment("단가");

            entity.HasOne(d => d.MaintenenceHistoryTb).WithMany(p => p.StoreTbs).HasConstraintName("fk_store_tb_maintenence_history_tb1");

            entity.HasOne(d => d.MaintenenceMaterialTb).WithMany(p => p.StoreTbs).HasConstraintName("fk_maintenence_material_tb");

            entity.HasOne(d => d.MaterialTb).WithMany(p => p.StoreTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_store_tb_material_tb1");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.StoreTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PLACE_202407231358");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.StoreTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ROOM_202407231358");
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

        modelBuilder.Entity<UseMaintenenceMaterialTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.MaintenanceTb).WithMany(p => p.UseMaintenenceMaterialTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceTB_20240906_1151");

            entity.HasOne(d => d.MaterialTb).WithMany(p => p.UseMaintenenceMaterialTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialTB_20240906_1150");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.UseMaintenenceMaterialTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlaceTB_20240906_1237");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.UseMaintenenceMaterialTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomTB_20240906_1150");
        });

        modelBuilder.Entity<UsersTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("사용자 인덱스");
            entity.Property(e => e.AdminYn).HasComment("관리자 여부");
            entity.Property(e => e.AlarmYn).HasComment("알람여부");
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
            entity.Property(e => e.Job).HasComment("직책");
            entity.Property(e => e.Name).HasComment("이름");
            entity.Property(e => e.Password).HasComment("비밀번호");
            entity.Property(e => e.PermBasic).HasComment("기본정보 권한");
            entity.Property(e => e.PermBeauty).HasComment("미화 권한");
            entity.Property(e => e.PermConstruct).HasComment("건축관리 권한");
            entity.Property(e => e.PermElec).HasComment("전기관리 권한");
            entity.Property(e => e.PermEnergy).HasComment("에너지관리 권한");
            entity.Property(e => e.PermFire).HasComment("소방관리 권한");
            entity.Property(e => e.PermLift).HasComment("승강관리 권한");
            entity.Property(e => e.PermMachine).HasComment("기계관리 권한");
            entity.Property(e => e.PermMaterial).HasComment("자재관리 권한");
            entity.Property(e => e.PermNetwork).HasComment("통신연동 권한");
            entity.Property(e => e.PermSecurity).HasComment("보안 권한");
            entity.Property(e => e.PermUser).HasComment("사용자 관리 권한");
            entity.Property(e => e.PermVoc).HasComment("민원관리 권한");
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
            entity.Property(e => e.Code).HasComment("VOC코드_민원조회용");
            entity.Property(e => e.CompleteDt).HasComment("완료시간");
            entity.Property(e => e.Content).HasComment("민원내용");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.DurationDt).HasComment("소요시간");
            entity.Property(e => e.Image1).HasComment("이미지");
            entity.Property(e => e.Image2).HasComment("이미지");
            entity.Property(e => e.Image3).HasComment("이미지");
            entity.Property(e => e.Phone).HasComment("전화번호");
            entity.Property(e => e.ReplyYn).HasComment("답변회신여부");
            entity.Property(e => e.Status).HasComment("민원처리상태");
            entity.Property(e => e.Title).HasComment("민원제목");
            entity.Property(e => e.Type).HasComment("0 : 미분류\r\n1 : 기계\r\n2 : 전기\r\n3 : 승강\r\n4 : 소방\r\n5 : 건축\r\n6 : 통신\r\n7 : 미화\r\n8 : 보안");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.VocTbs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("building_tb_202407250842");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    /// <summary>
    /// 쿼리스트링 사용
    /// </summary>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.DefaultTypeMapping<MaterialInventory>();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
