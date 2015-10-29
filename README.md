# VermintideBundleTool
Warhammer: End Times - Vermintide bundle extraction tool.

This tool has two functions:

1. Renaming the files in the *bundle* directory to their original names. This is done via a reverse lookup table called *dictionary.txt*.
2. Unpacking the content of *bundle* files. Bundle files are the files without file extensions in the *bundle* directory.

## Requirements
```
Microsoft .NET Framework 4.5
```

## Usage
```
VermintideBundleTool.exe verb options [optional options]
```

## Options
```
-i|input (required) Input directory or input file(s)
-o|output           Output directory
--verbose           Prints all messages to standard output
```

### Verbs
Renaming is done via the *rename*-verb.
```
VermintideBundleTool.exe rename -i path_to_bundle_directory
VermintideBundleTool.exe rename -i path_to_bundle_directory -o path_to_output_directory
```

Unpacking is done via the *unpack*-verb.
```
VermintideBundleTool.exe unpack -i path_to_bundle_file
VermintideBundleTool.exe unpack -i path_to_bundle_file path_to_another_bundle_file ...
VermintideBundleTool.exe unpack -i path_to_bundle_file -o path_to_output_directory
```

## Remarks
Repacking is not planned, since logging in with modified files will probably not be possible.

The dictionary is not yet complete. Files with names which cannot be looked up correctly will not be renamed.
