# MC-ABP
Automatic Block Properties; a small command-line application for generating simple/advanced block.properties file with extended metadata.

## Simple Usage
The "simple" generator creates an includable GLSL file containing a list of preprocessor defines that are generated from comments in the block.properties file.

```
MC-ABP simple block.properties blocks.glsl
```
- `block.properties` The filename of the block properties file to scan.
- `blocks.glsl` The filename of the GLSL defines file to generate.

Example block.properties file:
```properties
# BLOCK_TORCH
block.100=torch lantern

# BLOCK_SOUL_TORCH
block.101=soul_torch soul_lantern

# BLOCK_MAGMA
block.102=magma_block
```

The generated blocks.glsl file:
```glsl
#define BLOCK_TORCH 100
#define BLOCK_SOUL_TORCH 101
#define BLOCK_MAGMA 102
```

## Advanced Usage
```
MC-ABP advanced blocks.json block.properties blocks.glsl
```
This feature is still a work-in-progress.
