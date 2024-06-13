# APM [Automated Property Manager]
A small command-line application for automatically generating, and optionally numbering, block/item/entity properties files for Minecraft Java shaders.

## Automatic Defines
The most basic advantage to using APM is to have numbered defines automatically generated for you. This makes refering the blocks in code simpler, since you can reference them by name instead of hard-coded numbers. Using an example block.properties file:
```properties
#= BLOCK_TORCH
block.100=torch lantern

#= BLOCK_SOUL_TORCH
block.101=soul_torch soul_lantern

#= BLOCK_MAGMA
block.102=magma_block
```

and running the following command:
```sh
APM block
```

A blocks.glsl file will be generated containing the following defines:
```glsl
#define BLOCK_TORCH 100
#define BLOCK_SOUL_TORCH 101
#define BLOCK_MAGMA 102
```

## Automatic Numbering
A slightly more advanced use-case is to use APM to also automatically number blocks. This makes managing ID's much simpler, as adding/removing items no longer requires updating indices. Using an example block.template.properties file:
```properties
#= BLOCK_TORCH
block.200=torch lantern

#= BLOCK_SOUL_TORCH
block.*=soul_torch soul_lantern

#= BLOCK_MAGMA
block.*=magma_block
```

Running the following command will genrate two files. A final block.properties file for Optifine/Iris, as well as the blocks.glsl file from the prior example. Example command:
```sh
MC-APM block -t 'block.template.properties'
```

The generated block.properties file:
```properties
block.100=torch lantern
block.101=soul_torch soul_lantern
block.102=magma_block
```

and the same blocks.glsl file as before:
```glsl
#define BLOCK_TORCH 100
#define BLOCK_SOUL_TORCH 101
#define BLOCK_MAGMA 102
```

## Property Groups
Lists of objects can be reused via property groups.
```properties
group.candles=candle black_candle blue_candle brown_candle cyan_candle gray_candle green_candle light_blue_candle light_gray_candle lime_candle magenta_candle orange_candle pink_candle purple_candle red_candle white_candle yellow_candle

#= BLOCK_CANDLES_1
block.*=[candles]:candles=1:lit=false

#= BLOCK_CANDLES_2
block.*=[candles]:candles=2:lit=false

#= BLOCK_CANDLES_3
block.*=[candles]:candles=3:lit=false

#= BLOCK_CANDLES_4
block.*=[candles]:candles=4:lit=false
```
