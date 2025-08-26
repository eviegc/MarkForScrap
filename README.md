# MarkForScrap

A Risk of Rain 2 mod that allows you to mark items in your inventory that will be automagically scrapped the next time you use a scrapper.

See [the README for the mod itself](./Thunderstore/README.md) for usage instructions. Mod installation should be done via the [Thunderstore page](https://thunderstore.io/package/eviegc/MarkForScrap/).

The following instructions are for the development/building of the mod, and can be ignored if you just want to play with it!

## Setup

### UNetWeaver

Build steps require [UNetWeaver](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/C%23-Programming/Networking/UNet/#unetweaver) which is **not included** in this repo.

You'll need to download [NetworkWeaver.zip](https://github.com/user-attachments/files/17396610/NetworkWeaver.zip) (from the [RoR2 wiki page on UNetWeaver](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/C%23-Programming/Networking/UNet/#how-to-patch)), extract it, and place the folder at `[repo root]/tools/NetworkWeaver`, such that the `Unity.UNetWeaver.exe` is located at `[repo root]/tools/NetworkWeaver/Unity.UNetWeaver.exe`.

## Building

The mod uses [UNet](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/C%23-Programming/Networking/UNet/) for networking, and as such needs to be patched with [UNetWeaver](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/C%23-Programming/Networking/UNet/#unetweaver) ([see above for setup](#unetweaver)). This is done automatically as part of the build.

Build either with your IDE or something like:

```powershell
dotnet build MarkForScrap.sln /target:MarkForScrap /p:Configuration=[Debug|Release] /p:Platform="Any CPU"
```

This will output built DLLs to `MarkForScrap/bin/[Debug|Release]/netstandard2.1/Patched/MarkForScrap.dll`.

## Publishing

To publish to Thunderstore, you can use the [thunderstore-cli](https://github.com/thunderstore-io/thunderstore-cli). Config for this can be found in [Thunderstore/thunderstore.toml](Thunderstore/thunderstore.toml).

To publish, first perform a `Release` build, which will output the required mod `zip` file in `Thunderstore/build/[mod dependency string].zip`:

```powershell
dotnet build MarkForScrap.sln /target:MarkForScrap /p:Configuration=Release /p:Platform="Any CPU"
```

Or if creating the `zip` manually:

```powershell
tcli build --config-path Thunderstore\thunderstore.toml --package-version [version]
```

Then to actually push this `zip` to Thunderstore:

```powershell
tcli publish --config-path Thunderstore\thunderstore.toml --file Thunderstore\build\[mod dependency string].zip --token [token]
```

Where `[token]` is a Thunderstore service account token for the associated team.
