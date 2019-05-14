#include "MangoShared.h"
#include "MangoError.h"
#include "MangoBios.h"
#include "MangoBoards.h"

const int MangoBoards_Version = MANGOBOARDS_VERSION;

/* emifa global control */
#define EMIFA_GBLCTL	0x01800000
/* emifa chip enable space control 1 */
#define EMIFA_CECTL1	0x01800004
/* emifa chip enable space control 0 */
#define EMIFA_CECTL0	0x01800008
/* emifa chip enable space control 2 */
#define EMIFA_CECTL2	0x01800010
/* emifa chip enable space control 3 */
#define EMIFA_CECTL3	0x01800014
/* emifa sdram control */
#define EMIFA_SDCTL 	0x01800018
/* emifa sdram timing */
#define EMIFA_SDTIM 	0x0180001c
/* emifa sdram extension */
#define EMIFA_SDEXT 	0x01800020
/* emifa chip enable space secondary control 1 */
#define EMIFA_CESEC1	0x01800044
/* emifa chip enable space secondary control 0 */
#define EMIFA_CESEC0	0x01800048
/* emifa chip enable space secondary control 2 */
#define EMIFA_CESEC2	0x01800050
/* emifa chip enable space secondary control 3 */
#define EMIFA_CESEC3	0x01800054

/* emifb global control */
#define EMIFB_GBLCTL	0x01A80000
/* emifb chip enable space control 1 */
#define EMIFB_CECTL1	0x01A80004
/* emifb chip enable space control 0 */
#define EMIFB_CECTL0	0x01A80008
/* emifb chip enable space control 2 */
#define EMIFB_CECTL2	0x01A80010
/* emifb chip enable space control 3 */
#define EMIFB_CECTL3	0x01A80014
/* emifb sdram control */
#define EMIFB_SDCTL 	0x01A80018
/* emifb sdram timing */
#define EMIFB_SDTIM 	0x01A8001c
/* emifb sdram extension */
#define EMIFB_SDEXT 	0x01A80020
/* emifb chip enable space secondary control 1 */
#define EMIFB_CESEC1	0x01A80044
/* emifb chip enable space secondary control 0 */
#define EMIFB_CESEC0	0x01A80048
/* emifb chip enable space secondary control 2 */
#define EMIFB_CESEC2	0x01A80050
/* emifb chip enable space secondary control 3 */
#define EMIFB_CESEC3	0x01A80054

/* Seagull PMC */
static emif_reg_t SEAGULL_PMC_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x000020E4},
	{EMIFA_CECTL0, 0xFFFFFFD3},
	{EMIFA_CECTL1, 0xFFFFFF23},
	{EMIFA_CECTL2, 0xFFFFFF23},
	{EMIFA_CECTL3, 0xFFFFFF23},
	{EMIFA_SDCTL,  0x57117000},
	{EMIFA_SDTIM,  0x005DC7E0},
	{EMIFA_SDEXT,  0x00000D2B},
	{EMIFA_CESEC0, 0x00000002},
	{EMIFA_CESEC1, 0x00000002},
	{EMIFA_CESEC2, 0x00000002},
	{EMIFA_CESEC3, 0x00000002}
};

static emif_init_t SEAGULL_PMC_EMIF_INITS[] = {
	{sizeof(SEAGULL_PMC_EMIF_REGS) / sizeof(emif_reg_t), SEAGULL_PMC_EMIF_REGS}
};

static emif_init_array_t SEAGULL_PMC_EMIF_INIT_ARRAY = {
	sizeof(SEAGULL_PMC_EMIF_INITS) / sizeof(emif_init_t),
	SEAGULL_PMC_EMIF_INITS
};

static device_description_t SEAGULL_PMC_DEVICE_DESCRIPTIONS[] = {
	{C64_PCI, {0,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t SEAGULL_PMC_BOARD_DESCRIPTIONS[] = {
	sizeof(SEAGULL_PMC_DEVICE_DESCRIPTIONS) / sizeof(device_description_t), SEAGULL_PMC_DEVICE_DESCRIPTIONS
};

static board_description_array_t SEAGULL_PMC_BOARD_DESCRIPTION_ARRAY = {
	sizeof(SEAGULL_PMC_BOARD_DESCRIPTIONS) / sizeof(board_description_t),
	SEAGULL_PMC_BOARD_DESCRIPTIONS
};

board_footprint_t SEAGULL_PMC_BOARD = {
	"Seagull PMC",
	MANGOCARD_SEAGULL_PMC,
	&SEAGULL_PMC_BOARD_DESCRIPTION_ARRAY,
	&SEAGULL_PMC_EMIF_INIT_ARRAY
};

/* Seagull PCI */
static emif_reg_t SEAGULL_PCI_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x000820E4},
	{EMIFA_CECTL0, 0xFFFFFFD3},
	{EMIFA_CECTL1, 0xFFFFFF23},
	{EMIFA_CECTL2, 0xFFFFFF23},
	{EMIFA_CECTL3, 0xFFFFFF23},
	{EMIFA_SDCTL,  0x57117000},
	{EMIFA_SDTIM,  0x005DC7E0},
	{EMIFA_SDEXT,  0x00000D2B},
	{EMIFA_CESEC0, 0x00000002},
	{EMIFA_CESEC1, 0x00000002},
	{EMIFA_CESEC2, 0x00000002},
	{EMIFA_CESEC3, 0x00000002}
};

static emif_init_t SEAGULL_PCI_EMIFS[] = {
	{sizeof(SEAGULL_PCI_EMIF_REGS) / sizeof(emif_reg_t),SEAGULL_PCI_EMIF_REGS}
};

static emif_init_array_t SEAGULL_PCI_EMIF_INIT_ARRAY = {
	sizeof(SEAGULL_PCI_EMIFS) / sizeof(emif_init_t),
	SEAGULL_PCI_EMIFS
};

static device_description_t SEAGULL_PCI_DEVICE_DESCRIPTIONS[] = {
	{C64_PCI, {1,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t SEAGULL_PCI_BOARD_DESCRIPTIONS[] = {
	sizeof(SEAGULL_PCI_DEVICE_DESCRIPTIONS) / sizeof(device_description_t), SEAGULL_PCI_DEVICE_DESCRIPTIONS
};

static board_description_array_t SEAGULL_PCI_BOARD_DESCRIPTION_ARRAY = {
	sizeof(SEAGULL_PCI_BOARD_DESCRIPTIONS) / sizeof(board_description_t), SEAGULL_PCI_BOARD_DESCRIPTIONS
};

board_footprint_t SEAGULL_PCI_BOARD = {
	"Seagull PCI",
	MANGOCARD_SEAGULL_PCI,
	&SEAGULL_PCI_BOARD_DESCRIPTION_ARRAY,
	&SEAGULL_PCI_EMIF_INIT_ARRAY
};

/* Seagull PC104-Plus (Phoenix) */
static emif_reg_t SEAGULL_PC104_PLUS_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x00052078},
	{EMIFA_CECTL0, 0xFFFFFFD3},
	{EMIFA_CECTL1, 0x11D29202},
	{EMIFA_CECTL2, 0x22A28A22},
	{EMIFA_CECTL3, 0x22A28A22},
	{EMIFA_SDCTL,  0x57115000},
	{EMIFA_SDTIM,  0x0000081B},
	{EMIFA_SDEXT,  0x001FAF4D},
	{EMIFA_CESEC0, 0x00000002},
	{EMIFA_CESEC1, 0x00000002},
	{EMIFA_CESEC2, 0x00000002},
	{EMIFA_CESEC3, 0x00000073}
};

static emif_init_t SEAGULL_PC104_PLUS_EMIFS[] = {
	{sizeof(SEAGULL_PC104_PLUS_EMIF_REGS) / sizeof(emif_reg_t),SEAGULL_PC104_PLUS_EMIF_REGS}
};

static emif_init_array_t SEAGULL_PC104_PLUS_EMIF_INIT_ARRAY = {
	sizeof(SEAGULL_PC104_PLUS_EMIFS) / sizeof(emif_init_t),
	SEAGULL_PC104_PLUS_EMIFS
};

static device_description_t SEAGULL_PC104_PLUS_DEVICE_DESCRIPTIONS[] = {
	{C64_PCI, {0,0,0, 0x104c, 0x9065, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,1,0, 0x104c, 0x9065, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t SEAGULL_PC104_PLUS_BOARD_DESCRIPTIONS[] = {
	sizeof(SEAGULL_PC104_PLUS_DEVICE_DESCRIPTIONS) / sizeof(device_description_t), SEAGULL_PC104_PLUS_DEVICE_DESCRIPTIONS
};

static board_description_array_t SEAGULL_PC104_PLUS_BOARD_DESCRIPTION_ARRAY = {
	sizeof(SEAGULL_PC104_PLUS_BOARD_DESCRIPTIONS) / sizeof(board_description_t), SEAGULL_PC104_PLUS_BOARD_DESCRIPTIONS
};

board_footprint_t SEAGULL_PC104_PLUS_BOARD = {
	"Seagull PC104 Plus",
	MANGOCARD_SEAGULL_PC104_PLUS,
	&SEAGULL_PC104_PLUS_BOARD_DESCRIPTION_ARRAY,
	&SEAGULL_PC104_PLUS_EMIF_INIT_ARRAY
};

/* Raven-C */
static emif_reg_t RAVENC_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x000520E4},
	{EMIFA_CECTL0, 0xFFFFFFD3},
	{EMIFA_CECTL1, 0xFFFFFF03},
	{EMIFA_CECTL2, 0x2162C5E2},
	{EMIFA_CECTL3, 0x30b4c811},
	{EMIFA_SDCTL,  0x57117000},
	{EMIFA_SDTIM,  0x005DC7E0},
	{EMIFA_SDEXT,  0x00000D2B},
	{EMIFA_CESEC0, 0x00000002},
	{EMIFA_CESEC1, 0x00000002},
	{EMIFA_CESEC2, 0x00000073},
	{EMIFA_CESEC3, 0x00000073}
};

static emif_init_t RAVENC_EMIFS[] = {
	{sizeof(RAVENC_EMIF_REGS) / sizeof(emif_reg_t),RAVENC_EMIF_REGS}
};

static emif_init_array_t RAVENC_EMIF_INIT_ARRAY = {
	sizeof(RAVENC_EMIFS) / sizeof(emif_init_t),
	RAVENC_EMIFS
};

static device_description_t RAVENC_DEVICE_DESCRIPTIONS[] = {
	{C64_PCI, {0,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t RAVENC_BOARD_DESCRIPTIONS[] = {
	sizeof(RAVENC_DEVICE_DESCRIPTIONS) / sizeof(device_description_t), RAVENC_DEVICE_DESCRIPTIONS
};

static board_description_array_t RAVENC_BOARD_DESCRIPTION_ARRAY = {
	sizeof(RAVENC_BOARD_DESCRIPTIONS) / sizeof(board_description_t), RAVENC_BOARD_DESCRIPTIONS
};

board_footprint_t RAVENC_BOARD = {
	"Raven C",
	MANGOCARD_RAVENC,
	&RAVENC_BOARD_DESCRIPTION_ARRAY,
	&RAVENC_EMIF_INIT_ARRAY
};

/* Raven-V */
static emif_reg_t RAVENV_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x000520E4},
	{EMIFA_CECTL0, 0xFFFFFFD3},
	{EMIFA_CECTL1, 0xFFFFFF03},
	{EMIFA_CECTL2, 0x2162c542},
	{EMIFA_CECTL3, 0x30b4c811},
	{EMIFA_SDCTL,  0x57117000},
	{EMIFA_SDTIM,  0x005DC7E0},
	{EMIFA_SDEXT,  0x00000D2B},
	{EMIFA_CESEC0, 0x00000002},
	{EMIFA_CESEC1, 0x00000002},
	{EMIFA_CESEC2, 0x00000073},
	{EMIFA_CESEC3, 0x00000073}
};

static emif_init_t RAVENV_EMIFS[] = {
	{sizeof(RAVENV_EMIF_REGS) / sizeof(emif_reg_t),RAVENV_EMIF_REGS}
};

static emif_init_array_t RAVENV_EMIF_INIT_ARRAY = {
	sizeof(RAVENV_EMIFS) / sizeof(emif_init_t),
	RAVENV_EMIFS
};

static device_description_t RAVENV_DEVICE_DESCRIPTIONS[] = {
	{C64_PCI, {0,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,4,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,8,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,12,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t RAVENV_BOARD_DESCRIPTIONS[] = {
	sizeof(RAVENV_DEVICE_DESCRIPTIONS) / sizeof(device_description_t), RAVENV_DEVICE_DESCRIPTIONS
};

static board_description_array_t RAVENV_BOARD_DESCRIPTION_ARRAY = {
	sizeof(RAVENV_BOARD_DESCRIPTIONS) / sizeof(board_description_t), RAVENV_BOARD_DESCRIPTIONS
};

board_footprint_t RAVENV_BOARD = {
	"Raven V",
	MANGOCARD_RAVENV,
	&RAVENV_BOARD_DESCRIPTION_ARRAY,
	&RAVENV_EMIF_INIT_ARRAY
};

/* Bluejay */
static emif_reg_t BLUEJAY_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x000220A4},
	{EMIFA_CECTL0, 0xFFFFFFD3},
	{EMIFA_CECTL1, 0xFFFFFF43},
	{EMIFA_CECTL2, 0xFFFFFF43},
	{EMIFA_CECTL3, 0xFFFFFF43},
	{EMIFA_SDCTL,  0x63116000},
	{EMIFA_SDTIM,  0x00ddc250},
	{EMIFA_SDEXT,  0x00024D2B},
	{EMIFA_CESEC0, 0x00000033},
	{EMIFA_CESEC1, 0x00000033},
	{EMIFA_CESEC2, 0x00000033},
	{EMIFA_CESEC3, 0x00000033}
};

static emif_init_t BLUEJAY_EMIFS[] = {
	{sizeof(BLUEJAY_EMIF_REGS) / sizeof(emif_reg_t),BLUEJAY_EMIF_REGS}
};

static emif_init_array_t BLUEJAY_EMIF_INIT_ARRAY = {
	sizeof(BLUEJAY_EMIFS) / sizeof(emif_init_t),
	BLUEJAY_EMIFS
};

static device_description_t BLUEJAY_DEVICE_DESCRIPTIONS_0[] = {
	{C64_PCI, {2,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,5,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {2,4,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {2,8,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,9,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,13,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {2,12,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,7,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,6,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,10,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,11,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,15,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,14,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static device_description_t BLUEJAY_DEVICE_DESCRIPTIONS_1[] = {
	{C64_PCI, {1,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,5,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,4,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,8,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,9,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,13,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,12,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,7,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,6,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,10,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,11,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,15,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,14,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t BLUEJAY_BOARD_DESCRIPTIONS[] = {
	sizeof(BLUEJAY_DEVICE_DESCRIPTIONS_0) / sizeof(device_description_t), BLUEJAY_DEVICE_DESCRIPTIONS_0,
	sizeof(BLUEJAY_DEVICE_DESCRIPTIONS_1) / sizeof(device_description_t), BLUEJAY_DEVICE_DESCRIPTIONS_1
};

static board_description_array_t BLUEJAY_BOARD_DESCRIPTION_ARRAY = {
	sizeof(BLUEJAY_BOARD_DESCRIPTIONS) / sizeof(board_description_t), BLUEJAY_BOARD_DESCRIPTIONS
};

board_footprint_t BLUEJAY_BOARD = {
	"Bluejay",
	MANGOCARD_BLUEJAY,
	&BLUEJAY_BOARD_DESCRIPTION_ARRAY,
	&BLUEJAY_EMIF_INIT_ARRAY
};

static emif_reg_t BLUEJAY_RTM_C67_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x00003078},
	{EMIFA_CECTL0, 0xFFFFFF33},
	{EMIFA_CECTL1, 0xFFFFFF43},
	{EMIFA_CECTL2, 0xFFFFFF43},
	{EMIFA_CECTL3, 0xFFFFFF43},
	{EMIFA_SDCTL,  0x53116000},
	{EMIFA_SDTIM,  0x00000618},
	{EMIFA_SDEXT,  0x00040129}
};

/* Bluejay with RTM*/
static emif_init_t BLUEJAY_RTM_EMIFS[] = {
	{sizeof(BLUEJAY_EMIF_REGS) / sizeof(emif_reg_t),BLUEJAY_EMIF_REGS},
	{sizeof(BLUEJAY_RTM_C67_EMIF_REGS) / sizeof(emif_reg_t),BLUEJAY_RTM_C67_EMIF_REGS}
};

static emif_init_array_t BLUEJAY_RTM_EMIF_INIT_ARRAY = {
	sizeof(BLUEJAY_RTM_EMIFS) / sizeof(emif_init_t),
	BLUEJAY_RTM_EMIFS
};

static device_description_t BLUEJAY_RTM_DEVICE_DESCRIPTIONS_0[] = {
	{BRIDGE, {2,5,0, 0x104c, 0xac60, 0, 0, 0xffff, 0xffff, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,5,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,4,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,8,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,9,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,13,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,12,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,7,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,6,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,10,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,11,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,15,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,14,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C67_HPI, {0, 0, 16, 1, END_DEVICE_PARAMETERS_FLAG}},
	{C64_HPI, {0, 1, 16, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {2,4,0, 0x104c, 0x9065, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static device_description_t BLUEJAY_RTM_DEVICE_DESCRIPTIONS_1[] = {
	{BRIDGE, {3,5,0, 0x104c, 0xac60, 0, 0, 0xffff, 0xffff, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,0,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {5,1,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {5,5,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,4,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,8,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {5,9,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {5,13,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {1,12,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,2,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,3,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,7,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,6,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,10,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,11,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {4,15,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {0,14,0, 0x104c, 0xa106, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C67_HPI, {0, 0, 16, 1, END_DEVICE_PARAMETERS_FLAG}},
	{C64_HPI, {0, 1, 16, 0, END_DEVICE_PARAMETERS_FLAG}},
	{C64_PCI, {3,4,0, 0x104c, 0x9065, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t BLUEJAY_RTM_BOARD_DESCRIPTIONS[] = {
	sizeof(BLUEJAY_RTM_DEVICE_DESCRIPTIONS_0) / sizeof(device_description_t), BLUEJAY_RTM_DEVICE_DESCRIPTIONS_0,
	sizeof(BLUEJAY_RTM_DEVICE_DESCRIPTIONS_1) / sizeof(device_description_t), BLUEJAY_RTM_DEVICE_DESCRIPTIONS_1
};

static board_description_array_t BLUEJAY_RTM_BOARD_DESCRIPTION_ARRAY = {
	sizeof(BLUEJAY_RTM_BOARD_DESCRIPTIONS) / sizeof(board_description_t), BLUEJAY_RTM_BOARD_DESCRIPTIONS
};

board_footprint_t BLUEJAY_RTM_BOARD = {
	"Bluejay RTM",
	MANGOCARD_BLUEJAY_RTM,
	&BLUEJAY_RTM_BOARD_DESCRIPTION_ARRAY,
	&BLUEJAY_RTM_EMIF_INIT_ARRAY
};

/* Phoenix-HX */
static emif_reg_t PHOENIXHX_EMIF_REGS[] = {
	{EMIFA_GBLCTL, 0x00052078},
	{EMIFA_CECTL0, 0xffffffd3},
	{EMIFA_CECTL1, 0x73a28e01},
	{EMIFA_CECTL2, 0xFFFFFFB3},
	{EMIFA_CECTL3, 0xFFFFFFB3},
	{EMIFA_SDCTL,  0x63229000},
	{EMIFA_SDTIM,  0x000005dc},
	{EMIFA_SDEXT,  0x0017bc89},
	{EMIFA_CESEC0, 0x00000002},
	{EMIFA_CESEC1, 0x00000002},
	{EMIFA_CESEC2, 0x00000002},
	{EMIFA_CESEC3, 0x00000073}
};

static emif_init_t PHOENIXHX_EMIFS[] = {
	{sizeof(PHOENIXHX_EMIF_REGS) / sizeof(emif_reg_t),PHOENIXHX_EMIF_REGS}
};

static emif_init_array_t PHOENIXHX_EMIF_INIT_ARRAY = {
	sizeof(PHOENIXHX_EMIFS) / sizeof(emif_init_t),
	PHOENIXHX_EMIFS
};

static device_description_t PHOENIXHX_DEVICE_DESCRIPTIONS[] = {
	{C64_PCI, {0,0,0, 0x104c, 0x9065, 0, 0, 0xffff, 0xffff, 0, 0, 0, END_DEVICE_PARAMETERS_FLAG}}
};

static board_description_t PHOENIXHX_BOARD_DESCRIPTIONS[] = {
	sizeof(PHOENIXHX_DEVICE_DESCRIPTIONS) / sizeof(device_description_t), PHOENIXHX_DEVICE_DESCRIPTIONS
};

static board_description_array_t PHOENIXHX_BOARD_DESCRIPTION_ARRAY = {
	sizeof(PHOENIXHX_BOARD_DESCRIPTIONS) / sizeof(board_description_t), PHOENIXHX_BOARD_DESCRIPTIONS
};

board_footprint_t PHOENIXHX_BOARD = {
	"Phoenix HX",
	MANGOCARD_PHOENIXHX,
	&PHOENIXHX_BOARD_DESCRIPTION_ARRAY,
	&PHOENIXHX_EMIF_INIT_ARRAY
};

board_footprint_t * MANGO_BOARDS[] = {
	&BLUEJAY_RTM_BOARD,
	&BLUEJAY_BOARD,
	&SEAGULL_PCI_BOARD,
	&SEAGULL_PMC_BOARD,
	&SEAGULL_PC104_PLUS_BOARD,
	&RAVENC_BOARD,
	&RAVENV_BOARD,
	&PHOENIXHX_BOARD
};

const int NUM_MANGO_BOARDS = (sizeof(MANGO_BOARDS) / sizeof(board_footprint_t *));
