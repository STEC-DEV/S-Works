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

    public virtual DbSet<BuildingGroupitemTb> BuildingGroupitemTbs { get; set; }

    public virtual DbSet<BuildingItemkeyTb> BuildingItemkeyTbs { get; set; }

    public virtual DbSet<BuildingItemvalueTb> BuildingItemvalueTbs { get; set; }

    public virtual DbSet<BuildingTb> BuildingTbs { get; set; }

    public virtual DbSet<CommentTb> CommentTbs { get; set; }

    public virtual DbSet<DepartmentTb> DepartmentTbs { get; set; }

    public virtual DbSet<EnergyMonthUsageTb> EnergyMonthUsageTbs { get; set; }

    public virtual DbSet<EnergyUsageTb> EnergyUsageTbs { get; set; }

    public virtual DbSet<FacilityTb> FacilityTbs { get; set; }

    public virtual DbSet<FloorTb> FloorTbs { get; set; }

    public virtual DbSet<MaterialTb> MaterialTbs { get; set; }

    public virtual DbSet<MeterItemTb> MeterItemTbs { get; set; }

    public virtual DbSet<MeterReaderTb> MeterReaderTbs { get; set; }

    public virtual DbSet<PlaceTb> PlaceTbs { get; set; }

    public virtual DbSet<RoomTb> RoomTbs { get; set; }

    public virtual DbSet<StoreTb> StoreTbs { get; set; }

    public virtual DbSet<UnitTb> UnitTbs { get; set; }

    public virtual DbSet<UserTb> UserTbs { get; set; }

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

            entity.Property(e => e.Id).HasComment("관리자 사업장 인덱스");
            entity.Property(e => e.AdminTbId).HasComment("(외래키) 관리자 테이블 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.PlaceId).HasComment("(외래키) 사업장 테이블 인덱스");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.AdminTb).WithMany(p => p.AdminPlaceTbs).HasConstraintName("fk_ADMIN_TB_has_PLACE_ADMIN_TB1");

            entity.HasOne(d => d.Place).WithMany(p => p.AdminPlaceTbs).HasConstraintName("fk_ADMIN_TB_has_PLACE_PLACE1");
        });

        modelBuilder.Entity<AdminTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("관리자 테이블 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.DepartmentTbId).HasComment("(외래키) 부서 테이블 인덱스");
            entity.Property(e => e.Type).HasComment("계정유형");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.UserTbId).HasComment("(외래키) 사용자 테이블 인덱스");

            entity.HasOne(d => d.DepartmentTb).WithMany(p => p.AdminTbs).HasConstraintName("fk_ADMIN_TB_DEPARTMENT_TB1");

            entity.HasOne(d => d.UserTb).WithMany(p => p.AdminTbs).HasConstraintName("fk_ADMIN_TB_USER_TB");
        });

        modelBuilder.Entity<AlarmTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("알람 테이블 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.UpdateDt).HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.UserTbId).HasComment("(외래키) 사용자 테이블 인덱스");
            entity.Property(e => e.VocTbId).HasComment("(외래키) 민원 테이블 인덱스");

            entity.HasOne(d => d.UserTb).WithMany(p => p.AlarmTbs).HasConstraintName("FK_USER_202406141623");

            entity.HasOne(d => d.VocTb).WithMany(p => p.AlarmTbs).HasConstraintName("FK_VOC_202406141624");
        });

        modelBuilder.Entity<BuildingGroupitemTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("그룹테이블 아이디");
            entity.Property(e => e.BuildingId).HasComment("건물테이블아이디(외래키)");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성시간");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제시간");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("명칭 _주차장");
            entity.Property(e => e.UpdateDt).HasComment("수정시간");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
        });

        modelBuilder.Entity<BuildingItemkeyTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("아이템키 테이블 아이디");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성시간");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제시간");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.GroupItemId).HasComment("그룹테이블 아이디");
            entity.Property(e => e.Itemkey).HasComment("아이템 이름 _전기차");
            entity.Property(e => e.UpdateDt).HasComment("수정시간");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.GroupItem).WithMany(p => p.BuildingItemkeyTbs).HasConstraintName("FK_GroupItem_202407041727");
        });

        modelBuilder.Entity<BuildingItemvalueTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("값 테이블 아이디");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성시간");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제시간");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.ItemKeyId).HasComment("키 테이블 인덱스");
            entity.Property(e => e.Itemvalue).HasComment("아이템의 값 몇개");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정시간");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.ItemKey).WithMany(p => p.BuildingItemvalueTbs).HasConstraintName("FK_ItemKey_202407041727");
        });

        modelBuilder.Entity<BuildingTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("건물 테이블 인덱스");
            entity.Property(e => e.Address).HasComment("주소");
            entity.Property(e => e.Basementfloornum).HasComment("지하층수");
            entity.Property(e => e.Basementheight).HasComment("지하깊이");
            entity.Property(e => e.Boiler).HasComment("보일러");
            entity.Property(e => e.BuildingCd).HasComment("건물코드");
            entity.Property(e => e.BuildingStruct).HasComment("건물구조");
            entity.Property(e => e.Buildingarea).HasComment("건축면적");
            entity.Property(e => e.Buildingheight).HasComment("건물높이");
            entity.Property(e => e.Cargoliftnum).HasComment("화물용 대수");
            entity.Property(e => e.CompletionDt).HasComment("준공년월");
            entity.Property(e => e.ConstComp).HasComment("시공업체");
            entity.Property(e => e.Coolcapacity).HasComment("냉방용량");
            entity.Property(e => e.Coolheatcapacity).HasComment("냉난방용량");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Eleccapacity).HasComment("전기용량");
            entity.Property(e => e.Elevwatercapacity).HasComment("고가수조");
            entity.Property(e => e.Faucetcapacity).HasComment("수전용량");
            entity.Property(e => e.Firerating).HasComment("소방등급");
            entity.Property(e => e.Floornum).HasComment("건물층수");
            entity.Property(e => e.Gascapacity).HasComment("가스용량");
            entity.Property(e => e.Generationcapacity).HasComment("발전용량");
            entity.Property(e => e.Grossfloorarea).HasComment("연면적");
            entity.Property(e => e.Groundarea).HasComment("지상면적");
            entity.Property(e => e.Groundfloornum).HasComment("지상층수");
            entity.Property(e => e.Groundheight).HasComment("지상높이");
            entity.Property(e => e.Heatcapacity).HasComment("난방용량");
            entity.Property(e => e.Image).HasComment("첨부파일");
            entity.Property(e => e.Innerparkingnum).HasComment("옥내대수");
            entity.Property(e => e.Landarea).HasComment("대지면적");
            entity.Property(e => e.Landscapearea).HasComment("조경면적");
            entity.Property(e => e.Liftnum).HasComment("승강기 대수");
            entity.Property(e => e.Mentoiletnum).HasComment("남자화장실개수");
            entity.Property(e => e.Name).HasComment("건물명");
            entity.Property(e => e.Outerparkingnum).HasComment("옥외대수");
            entity.Property(e => e.Parkingnum).HasComment("주차대수");
            entity.Property(e => e.Peopleliftnum).HasComment("인승용 대수");
            entity.Property(e => e.PlaceTbId).HasComment("(외래키) 사업장 인덱스");
            entity.Property(e => e.RoofStruct).HasComment("지붕구조");
            entity.Property(e => e.Rooftoparea).HasComment("옥상면적");
            entity.Property(e => e.Septictankcapacity).HasComment("정화조용량");
            entity.Property(e => e.Tel).HasComment("전화번호");
            entity.Property(e => e.Toiletnum).HasComment("화장실개수");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.Usage).HasComment("건물용도");
            entity.Property(e => e.Watercapacity).HasComment("급수용량");
            entity.Property(e => e.Waterdispenser).HasComment("냉온수기");
            entity.Property(e => e.Watertank).HasComment("저수조");
            entity.Property(e => e.Womentoiletnum).HasComment("여자화장실개수");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.BuildingTbs).HasConstraintName("fk_BUILDING_TB_PLACE_TB1");
        });

        modelBuilder.Entity<CommentTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("댓글 인덱스");
            entity.Property(e => e.Content).HasComment("댓글 내용");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Status).HasComment("처리 상태");
            entity.Property(e => e.UpdateDt).HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.VocTbid).HasComment("(외래키) 민원 인덱스");

            entity.HasOne(d => d.VocTb).WithMany(p => p.CommentTbs).HasConstraintName("VOC_202406211546");
        });

        modelBuilder.Entity<DepartmentTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("부서 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("부서명");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
        });

        modelBuilder.Entity<EnergyMonthUsageTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("에너지 월별 사용량 인덱스");
            entity.Property(e => e.Apr).HasComment("4월");
            entity.Property(e => e.Aug).HasComment("8월");
            entity.Property(e => e.CreateDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelDt).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.DelYn).HasDefaultValueSql("'0'");
            entity.Property(e => e.Dev).HasComment("12월");
            entity.Property(e => e.Feb).HasComment("2월");
            entity.Property(e => e.Jan).HasComment("1월");
            entity.Property(e => e.Jul).HasComment("7월");
            entity.Property(e => e.Jun).HasComment("6월");
            entity.Property(e => e.Mar).HasComment("3월");
            entity.Property(e => e.May).HasComment("5월");
            entity.Property(e => e.MeterReaderTbId).HasComment("(외래키)검침기 인덱스");
            entity.Property(e => e.Nov).HasComment("11월");
            entity.Property(e => e.Oct).HasComment("10월");
            entity.Property(e => e.Sep).HasComment("9월");
            entity.Property(e => e.UpdateDt).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.MeterReaderTb).WithMany(p => p.EnergyMonthUsageTbs).HasConstraintName("fk_ENERGY_MONTH_USAGE_TB_METER_READER_TB1");
        });

        modelBuilder.Entity<EnergyUsageTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("에너지 사용량 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.MeterDt).HasComment("검침일자");
            entity.Property(e => e.MeterItemTbId).HasComment("(외래키)검침항목 인덱스");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.Usage).HasComment("사용량");

            entity.HasOne(d => d.MeterItemTb).WithMany(p => p.EnergyUsageTbs).HasConstraintName("fk_ENERGY_USAGE_TB_METER_ITEM_TB1");
        });

        modelBuilder.Entity<FacilityTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("설비 인덱스");
            entity.Property(e => e.Category).HasComment("카테고리(기계,전기..)");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Ea)
                .HasDefaultValueSql("'0'")
                .HasComment("수량");
            entity.Property(e => e.FacCreateDt).HasComment("설치년월");
            entity.Property(e => e.FacUpdateDt).HasComment("교체년월");
            entity.Property(e => e.Lifespan).HasComment("내용연수");
            entity.Property(e => e.Name).HasComment("설비명칭");
            entity.Property(e => e.RoomTbid).HasComment("(외래키)공간 인덱스");
            entity.Property(e => e.StandardCapacity).HasComment("규격용량");
            entity.Property(e => e.StandardCapacityUnit).HasComment("규격용량단위");
            entity.Property(e => e.Type).HasComment("형식");
            entity.Property(e => e.UpdateDt).HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.FacilityTbs).HasConstraintName("FK_ROOM_202406211458");
        });

        modelBuilder.Entity<FloorTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("층 인덱스");
            entity.Property(e => e.BuildingTbId).HasComment("(외래키) 건물 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("층 이름");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.FloorTbs).HasConstraintName("fk_FLOOR_TB_BUILDING_TB1");
        });

        modelBuilder.Entity<MaterialTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("자재 인덱스");
            entity.Property(e => e.BuildingTbId).HasComment("(외래키) 건물 아이디");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DefaultLocation).HasComment("자재위치");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제여부");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Mfr).HasComment("제조사");
            entity.Property(e => e.Name).HasComment("자재명");
            entity.Property(e => e.PlaceTbId).HasComment("(외래키) 사업장 아이디");
            entity.Property(e => e.SafeNum).HasComment("안전재고수량");
            entity.Property(e => e.Standard).HasComment("규격");
            entity.Property(e => e.Unit).HasComment("단위");
            entity.Property(e => e.UpdateDt).HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.MaterialTbs).HasConstraintName("BuildingTbId_202406211648");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.MaterialTbs).HasConstraintName("PlaceTbId_202406211648");
        });

        modelBuilder.Entity<MeterItemTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("자동증가 인덱스");
            entity.Property(e => e.AccumUsage).HasComment("누적사용량");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.MeterItem).HasComment("검침항목");
            entity.Property(e => e.MeterReaderTbId).HasComment("(외래키)검침기 인덱스");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.MeterReaderTb).WithMany(p => p.MeterItemTbs).HasConstraintName("fk_METER_ITEM_TB_METER_READER_TB1");
        });

        modelBuilder.Entity<MeterReaderTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("자동증가 인덱스");
            entity.Property(e => e.BuildingTbId).HasComment("(외래키)건물인덱스");
            entity.Property(e => e.Category).HasComment("카테고리");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.MeterItem).HasComment("검침항목");
            entity.Property(e => e.Type).HasComment("계약종별");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.MeterReaderTbs).HasConstraintName("fk_METER_READER_TB_BUILDING_TB1");
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
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Name).HasComment("사업장명");
            entity.Property(e => e.Note).HasComment("비고");
            entity.Property(e => e.PermBeauty)
                .HasDefaultValueSql("'0'")
                .HasComment("미화메뉴 권한");
            entity.Property(e => e.PermConstruct)
                .HasDefaultValueSql("'0'")
                .HasComment("건축메뉴 권한");
            entity.Property(e => e.PermEnergy)
                .HasDefaultValueSql("'0'")
                .HasComment("에너지메뉴 권한");
            entity.Property(e => e.PermFire)
                .HasDefaultValueSql("'0'")
                .HasComment("소방메뉴 권한");
            entity.Property(e => e.PermLift)
                .HasDefaultValueSql("'0'")
                .HasComment("승강메뉴 권한");
            entity.Property(e => e.PermMachine)
                .HasDefaultValueSql("'0'")
                .HasComment("기계메뉴 권한");
            entity.Property(e => e.PermMaterial)
                .HasDefaultValueSql("'0'")
                .HasComment("자재메뉴 권한");
            entity.Property(e => e.PermNetwork)
                .HasDefaultValueSql("'0'")
                .HasComment("통신메뉴 권한");
            entity.Property(e => e.PermSecurity)
                .HasDefaultValueSql("'0'")
                .HasComment("보안메뉴 권한");
            entity.Property(e => e.PermVoc)
                .HasDefaultValueSql("'0'")
                .HasComment("민원 권한");
            entity.Property(e => e.PlaceCd).HasComment("사업장코드");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("계약상태");
            entity.Property(e => e.Tel).HasComment("전화번호");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
        });

        modelBuilder.Entity<RoomTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("공간 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.FloorTbId).HasComment("(외래키) 층 인덱스");
            entity.Property(e => e.Name).HasComment("공간명");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.FloorTb).WithMany(p => p.RoomTbs).HasConstraintName("fk_ROOM_TB_FLOOR_TB1");
        });

        modelBuilder.Entity<StoreTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("입출고 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.InOut).HasComment("입출고 구분");
            entity.Property(e => e.InoutDate).HasComment("입출고날짜");
            entity.Property(e => e.MaterialTbid).HasComment("(외래키)자재 인덱스");
            entity.Property(e => e.Num).HasComment("수량");
            entity.Property(e => e.Price).HasComment("금액");
            entity.Property(e => e.RoomTbid).HasComment("(외래키)공간 인덱스");
            entity.Property(e => e.UnitPrice).HasComment("단가");
            entity.Property(e => e.UpdateDt).HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.MaterialTb).WithMany(p => p.StoreTbs).HasConstraintName("FK_MATERIAL_202406211523");

            entity.HasOne(d => d.RoomTb).WithMany(p => p.StoreTbs).HasConstraintName("FK_ROOM_202406211523");
        });

        modelBuilder.Entity<UnitTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("단위 인덱스");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.PlaceTbId).HasComment("(외래키) 사업장 인덱스");
            entity.Property(e => e.Unit).HasComment("단위명");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.UnitTbs).HasConstraintName("fk_UNIT_PLACE_TB1");
        });

        modelBuilder.Entity<UserTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("사용자 인덱스");
            entity.Property(e => e.AdminYn)
                .HasDefaultValueSql("'0'")
                .HasComment("관리자 유무");
            entity.Property(e => e.AlramYn)
                .HasDefaultValueSql("'0'")
                .HasComment("알람 유무");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Email).HasComment("이메일");
            entity.Property(e => e.Image).HasComment("첨부파일");
            entity.Property(e => e.Job).HasComment("직급");
            entity.Property(e => e.Name).HasComment("이름");
            entity.Property(e => e.Password).HasComment("비밀번호");
            entity.Property(e => e.PermBasic)
                .HasDefaultValueSql("'0'")
                .HasComment("기본정보관리 권한");
            entity.Property(e => e.PermBeauty)
                .HasDefaultValueSql("'0'")
                .HasComment("미화 권한");
            entity.Property(e => e.PermConstruct)
                .HasDefaultValueSql("'0'")
                .HasComment("건축 권한");
            entity.Property(e => e.PermElec)
                .HasDefaultValueSql("'0'")
                .HasComment("전기 권한");
            entity.Property(e => e.PermEnergy)
                .HasDefaultValueSql("'0'")
                .HasComment("에너지관리 권한");
            entity.Property(e => e.PermFire)
                .HasDefaultValueSql("'0'")
                .HasComment("소방 권한");
            entity.Property(e => e.PermLift)
                .HasDefaultValueSql("'0'")
                .HasComment("승강 권한");
            entity.Property(e => e.PermMachine)
                .HasDefaultValueSql("'0'")
                .HasComment("기계 권한");
            entity.Property(e => e.PermMaterial)
                .HasDefaultValueSql("'0'")
                .HasComment("자재관리 권한");
            entity.Property(e => e.PermNetwork)
                .HasDefaultValueSql("'0'")
                .HasComment("통신 권한");
            entity.Property(e => e.PermSecurity)
                .HasDefaultValueSql("'0'")
                .HasComment("보안 권한");
            entity.Property(e => e.PermUser)
                .HasDefaultValueSql("'0'")
                .HasComment("사용자관리 권한");
            entity.Property(e => e.PermVoc)
                .HasDefaultValueSql("'0'")
                .HasComment("민원관리 권한");
            entity.Property(e => e.Phone).HasComment("전화번호");
            entity.Property(e => e.PlaceTbId).HasComment("(외래키) 사업장 인덱스");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("재직여부");
            entity.Property(e => e.UpdateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");
            entity.Property(e => e.UserId).HasComment("사용자 아이디");
            entity.Property(e => e.VocBeauty)
                .HasDefaultValueSql("'0'")
                .HasComment("미화민원 처리권한");
            entity.Property(e => e.VocConstruct)
                .HasDefaultValueSql("'0'")
                .HasComment("건축민원 처리권한");
            entity.Property(e => e.VocDefault)
                .HasDefaultValueSql("'0'")
                .HasComment("기타 처리권한");
            entity.Property(e => e.VocElec)
                .HasDefaultValueSql("'0'")
                .HasComment("전기민원 처리권한");
            entity.Property(e => e.VocFire)
                .HasDefaultValueSql("'0'")
                .HasComment("소방민원 처리권한");
            entity.Property(e => e.VocLift)
                .HasDefaultValueSql("'0'")
                .HasComment("승강민원 처리권한");
            entity.Property(e => e.VocMachine)
                .HasDefaultValueSql("'0'")
                .HasComment("기계민원 처리권한");
            entity.Property(e => e.VocNetwork)
                .HasDefaultValueSql("'0'")
                .HasComment("통신민원 처리권한");
            entity.Property(e => e.VocSecurity)
                .HasDefaultValueSql("'0'")
                .HasComment("보안민원 처리권한");

            entity.HasOne(d => d.PlaceTb).WithMany(p => p.UserTbs).HasConstraintName("fk_USER_TB_PLACE_TB1");
        });

        modelBuilder.Entity<VocTb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasComment("민원 인덱스");
            entity.Property(e => e.BuildingTbId).HasComment("(외래키) 건물 인덱스");
            entity.Property(e => e.CompleteTime).HasComment("완료시간");
            entity.Property(e => e.Content).HasComment("민원 내용");
            entity.Property(e => e.CreateDt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("생성일");
            entity.Property(e => e.CreateUser).HasComment("생성자");
            entity.Property(e => e.DelDt).HasComment("삭제일");
            entity.Property(e => e.DelUser).HasComment("삭제자");
            entity.Property(e => e.DelYn)
                .HasDefaultValueSql("'0'")
                .HasComment("삭제여부");
            entity.Property(e => e.Image1).HasComment("첨부파일_1");
            entity.Property(e => e.Image2).HasComment("첨부파일_2");
            entity.Property(e => e.Image3).HasComment("첨부파일_3");
            entity.Property(e => e.Name).HasComment("민원인 이름");
            entity.Property(e => e.Phone).HasComment("민원인 전화번호");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasComment("민원처리 상태");
            entity.Property(e => e.Title).HasComment("민원 제목");
            entity.Property(e => e.TotalTime).HasComment("소요시간");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'0'")
                .HasComment("민원유형");
            entity.Property(e => e.UpdateDt).HasComment("수정일");
            entity.Property(e => e.UpdateUser).HasComment("수정자");

            entity.HasOne(d => d.BuildingTb).WithMany(p => p.VocTbs).HasConstraintName("FK_BULDING_202406141619");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
