using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamTec.Server.Migrations
{
    /// <inheritdoc />
    public partial class Works : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "department_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "material_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false, comment: "자재 인덱스")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CATEGORY = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TYPE = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MATERIAL_CODE = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UNIT = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STANDARD = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MFR = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, comment: "제조사", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SAFE_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PLACE_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "place_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PLACE_CD = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CONTRACT_NUM = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TEL = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NOTE = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADDRESS = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CONTRACT_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    PERM_MACHINE = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_LIFT = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_FIRE = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_CONSTRUCT = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_NETWROK = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_BEAUTY = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_SECURITY = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_MATERIAL = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_ENERGY = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    PERM_VOC = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    CANCEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    STATUS = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'1'"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "building_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BUILDING_CD = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADDRESS = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TEL = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    USAGE = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CONST_COMP = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    COMPLETION_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    BUILDING_STRUCT = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ROOF_STRUCT = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GROSS_FLOOR_AREA = table.Column<float>(type: "float", nullable: true),
                    LAND_AREA = table.Column<float>(type: "float", nullable: true),
                    BUILDING_AREA = table.Column<float>(type: "float", nullable: true),
                    FLOOR_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    GROUND_FLOOR_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    BASEMENT_FLOOR_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    BUILDING_HEIGHT = table.Column<float>(type: "float", nullable: true),
                    GROUND_HEIGHT = table.Column<float>(type: "float", nullable: true),
                    BASEMENT_HEIGHT = table.Column<float>(type: "float", nullable: true),
                    PARKING_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    INNER_PARKING_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    OUTER_PARKING_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    ELEC_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    FAUCET_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    GENERATION_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    WATER_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    ELEV_WATER_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    WATER_TANK = table.Column<float>(type: "float", nullable: true),
                    GAS_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    BOILER = table.Column<float>(type: "float", nullable: true),
                    WATER_DISPENSER = table.Column<float>(type: "float", nullable: true),
                    LIFT_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    PEOPLE_LIFT_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    CARGO_LIFT_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    COOL_HEAT_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    HEAT_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    COOL_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    LANDSCAPE_AREA = table.Column<float>(type: "float", nullable: true),
                    GROUND_AREA = table.Column<float>(type: "float", nullable: true),
                    ROOFTOP_AREA = table.Column<float>(type: "float", nullable: true),
                    TOILET_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    MEN_TOILET_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    WOMEN_TOILET_NUM = table.Column<int>(type: "int(11)", nullable: true),
                    FIRE_RATING = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SEPTIC_TANK_CAPACITY = table.Column<float>(type: "float", nullable: true),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PLACE_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_BUILDING_TB_PLACE_TB1",
                        column: x => x.PLACE_TB_ID,
                        principalTable: "place_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "unit_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UNIT = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PLACE_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_UNIT_PLACE_TB1",
                        column: x => x.PLACE_TB_ID,
                        principalTable: "place_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "user_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PASSWORD = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JOB = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EMAIL = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PHONE = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PERM_BASIC = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_MACHINE = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_LIFT = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_FIRE = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_CONSTRUCT = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_NETWORK = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_BEAUTY = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_SECURITY = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_MATERIAL = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_ENERGY = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_USER = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    PERM_VOC = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    ADMIN_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    ALRAM_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    STATUS = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'1'"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PLACE_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_USER_TB_PLACE_TB1",
                        column: x => x.PLACE_TB_ID,
                        principalTable: "place_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "floor_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BUILDING_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_FLOOR_TB_BUILDING_TB1",
                        column: x => x.BUILDING_TB_ID,
                        principalTable: "building_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "meter_reader_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CATEGORY = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TYPE = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, comment: "계약종별", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    METER_ITEM = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BUILDING_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_METER_READER_TB_BUILDING_TB1",
                        column: x => x.BUILDING_TB_ID,
                        principalTable: "building_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "admin_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TYPE = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    USER_TB_ID = table.Column<int>(type: "int(11)", nullable: true),
                    DEPARTMENT_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_ADMIN_TB_DEPARTMENT_TB1",
                        column: x => x.DEPARTMENT_TB_ID,
                        principalTable: "department_tb",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "fk_ADMIN_TB_USER_TB",
                        column: x => x.USER_TB_ID,
                        principalTable: "user_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "room_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NAME = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FLOOR_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_ROOM_TB_FLOOR_TB1",
                        column: x => x.FLOOR_TB_ID,
                        principalTable: "floor_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "energy_month_usage_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JAN = table.Column<float>(type: "float", nullable: true),
                    FEB = table.Column<float>(type: "float", nullable: true),
                    MAR = table.Column<float>(type: "float", nullable: true),
                    APR = table.Column<float>(type: "float", nullable: true),
                    MAY = table.Column<float>(type: "float", nullable: true),
                    JUN = table.Column<float>(type: "float", nullable: true),
                    JUL = table.Column<float>(type: "float", nullable: true),
                    AUG = table.Column<float>(type: "float", nullable: true),
                    SEP = table.Column<float>(type: "float", nullable: true),
                    OCT = table.Column<float>(type: "float", nullable: true),
                    NOV = table.Column<float>(type: "float", nullable: true),
                    DEV = table.Column<float>(type: "float", nullable: true),
                    METER_READER_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_ENERGY_MONTH_USAGE_TB_METER_READER_TB1",
                        column: x => x.METER_READER_TB_ID,
                        principalTable: "meter_reader_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "meter_item_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    METER_ITEM = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, comment: "검침항목", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ACCUM_USAGE = table.Column<float>(type: "float", nullable: true),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    METER_READER_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_METER_ITEM_TB_METER_READER_TB1",
                        column: x => x.METER_READER_TB_ID,
                        principalTable: "meter_reader_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "admin_place_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADMIN_TB_ID = table.Column<int>(type: "int(11)", nullable: true),
                    PLACE_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_ADMIN_TB_has_PLACE_ADMIN_TB1",
                        column: x => x.ADMIN_TB_ID,
                        principalTable: "admin_tb",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "fk_ADMIN_TB_has_PLACE_PLACE1",
                        column: x => x.PLACE_ID,
                        principalTable: "place_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "store_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CATEGORY = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NUM = table.Column<int>(type: "int(11)", nullable: true),
                    UNIT_PRICE = table.Column<float>(type: "float", nullable: true, comment: "단가\\n"),
                    PRICE = table.Column<float>(type: "float", nullable: true, comment: "금액\\n"),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MATERIAL_TB_ID = table.Column<int>(type: "int(11)", nullable: true),
                    ROOM_TB_ID = table.Column<int>(type: "int(11)", nullable: true),
                    BUILDING_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_STORE_TB_BUILDING_TB1",
                        column: x => x.BUILDING_TB_ID,
                        principalTable: "building_tb",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "fk_STORE_TB_MATERIAL_TB1",
                        column: x => x.MATERIAL_TB_ID,
                        principalTable: "material_tb",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "fk_STORE_TB_ROOM_TB1",
                        column: x => x.ROOM_TB_ID,
                        principalTable: "room_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "energy_usage_tb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USAGE = table.Column<float>(type: "float", nullable: true),
                    METER_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    CREATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    CREATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    UPDATE_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DEL_YN = table.Column<sbyte>(type: "tinyint(4)", nullable: true, defaultValueSql: "'0'"),
                    DEL_DT = table.Column<DateTime>(type: "datetime", nullable: true),
                    DEL_USER = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    METER_ITEM_TB_ID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                    table.ForeignKey(
                        name: "fk_ENERGY_USAGE_TB_METER_ITEM_TB1",
                        column: x => x.METER_ITEM_TB_ID,
                        principalTable: "meter_item_tb",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "fk_ADMIN_TB_has_PLACE_ADMIN_TB1_idx",
                table: "admin_place_tb",
                column: "ADMIN_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_ADMIN_TB_has_PLACE_PLACE1_idx",
                table: "admin_place_tb",
                column: "PLACE_ID");

            migrationBuilder.CreateIndex(
                name: "fk_ADMIN_TB_DEPARTMENT_TB1_idx",
                table: "admin_tb",
                column: "DEPARTMENT_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_ADMIN_TB_USER_TB_idx",
                table: "admin_tb",
                column: "USER_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_BUILDING_TB_PLACE_TB1_idx",
                table: "building_tb",
                column: "PLACE_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_ENERGY_MONTH_USAGE_TB_METER_READER_TB1_idx",
                table: "energy_month_usage_tb",
                column: "METER_READER_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_ENERGY_USAGE_TB_METER_ITEM_TB1_idx",
                table: "energy_usage_tb",
                column: "METER_ITEM_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_FLOOR_TB_BUILDING_TB1_idx",
                table: "floor_tb",
                column: "BUILDING_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_METER_ITEM_TB_METER_READER_TB1_idx",
                table: "meter_item_tb",
                column: "METER_READER_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_METER_READER_TB_BUILDING_TB1_idx",
                table: "meter_reader_tb",
                column: "BUILDING_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_ROOM_TB_FLOOR_TB1_idx",
                table: "room_tb",
                column: "FLOOR_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_STORE_TB_BUILDING_TB1_idx",
                table: "store_tb",
                column: "BUILDING_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_STORE_TB_MATERIAL_TB1_idx",
                table: "store_tb",
                column: "MATERIAL_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_STORE_TB_ROOM_TB1_idx",
                table: "store_tb",
                column: "ROOM_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_UNIT_PLACE_TB1_idx",
                table: "unit_tb",
                column: "PLACE_TB_ID");

            migrationBuilder.CreateIndex(
                name: "fk_USER_TB_PLACE_TB1_idx",
                table: "user_tb",
                column: "PLACE_TB_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_place_tb");

            migrationBuilder.DropTable(
                name: "energy_month_usage_tb");

            migrationBuilder.DropTable(
                name: "energy_usage_tb");

            migrationBuilder.DropTable(
                name: "store_tb");

            migrationBuilder.DropTable(
                name: "unit_tb");

            migrationBuilder.DropTable(
                name: "admin_tb");

            migrationBuilder.DropTable(
                name: "meter_item_tb");

            migrationBuilder.DropTable(
                name: "material_tb");

            migrationBuilder.DropTable(
                name: "room_tb");

            migrationBuilder.DropTable(
                name: "department_tb");

            migrationBuilder.DropTable(
                name: "user_tb");

            migrationBuilder.DropTable(
                name: "meter_reader_tb");

            migrationBuilder.DropTable(
                name: "floor_tb");

            migrationBuilder.DropTable(
                name: "building_tb");

            migrationBuilder.DropTable(
                name: "place_tb");
        }
    }
}
