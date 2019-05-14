#ifndef __MANGOBOARDS_H__
#define __MANGOBOARDS_H__

#ifdef __cplusplus
extern "C" {
#endif

#define MANGOBOARDS_VERSION (4)
#define END_DEVICE_PARAMETERS_FLAG (0xfacedefl)
extern const int MangoBoards_Version;

typedef enum device_type_s{
	C64_PCI = 1,
	C64_HPI,
	C67_HPI,
	BRIDGE
} device_type_t;

typedef struct emif_reg_s{
	int addr;
	int val;
} emif_reg_t;

typedef struct emif_init_s{
	int num_emif_regs;
	emif_reg_t * emif_regs;
} emif_init_t;

typedef struct emif_init_array_s{
	int num_emif_inits;
	emif_init_t * emif_inits;
} emif_init_array_t;

typedef struct pci_location_s{
	int bus;
	int dev;
	int func;
} pci_location_t;

typedef struct pci_id_s{
	unsigned int vendor;
	unsigned int device;
	unsigned int subsystem_vendor;
	unsigned int subsystem;
} pci_id_t;

typedef struct pci_parameters_s{
	pci_location_t pci_location;
	pci_id_t pci_id;
	pci_id_t pci_id_mask;
} pci_parameters_t;

typedef struct bridge_parameters_s{
	pci_parameters_t pci_parameters;
	int flag;
} bridge_parameters_t;

typedef struct c64_pci_parameters_s{
	pci_parameters_t pci_parameters;
	int emif_init_index;
	int flag;
} c64_pci_parameters_t;

typedef struct hpi_parameters_s{
	int bridge_index;
	int device_index;
	int bus_width;
} hpi_parameters_t;

typedef struct c64_hpi_parameters_s{
	hpi_parameters_t hpi_parameters;
	int emif_init_index;
	int flag;
} c64_hpi_parameters_t;

typedef struct c67_hpi_parameters_s{
	hpi_parameters_t hpi_parameters;
	int emif_init_index;
	int flag;
} c67_hpi_parameters_t;

typedef union device_parameters_s{
	int values[sizeof(c64_pci_parameters_t)];
	c64_pci_parameters_t c64_pci_parameters;
	bridge_parameters_t bridge_parameters;
	c64_hpi_parameters_t c64_hpi_parameters;
	c67_hpi_parameters_t c67_hpi_parameters;
	pci_parameters_t pci_parameters;
	hpi_parameters_t hpi_parameters;
} device_parameters_t;

typedef struct device_description_s{
	device_type_t device_type;
	device_parameters_t device_parameters;
} device_description_t;

typedef struct board_description_s{
	int num_device_descriptions;
	device_description_t * device_descriptions;
} board_description_t;

typedef struct board_description_array_s{
	int num_board_descriptions;
	board_description_t * board_descriptions;
} board_description_array_t;

typedef struct board_footprint_s{
	char * name;
	MANGOCARD_card_t card;
	board_description_array_t * board_description_array;
	emif_init_array_t * emif_array;
} board_footprint_t;

extern board_footprint_t
	SEAGULL_PMC_BOARD,
	SEAGULL_PC104_PLUS_BOARD,
	SEAGULL_PCI_BOARD,
	RAVENC_BOARD,
	RAVENV_BOARD,
	BLUEJAY_BOARD,
	BLUEJAY_RTM_BOARD,
	PHOENIXHX_BOARD;

extern board_footprint_t * MANGO_BOARDS[];

extern const int NUM_MANGO_BOARDS;


#ifdef __cplusplus
}
#endif

#endif /* __MANGOBOARDS_H__ */
