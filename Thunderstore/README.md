# MarkForScrap

A mod that allows you to mark items in your inventory that will be automagically scrapped the next time you use a scrapper.

The goal with this mod is to be as close to vanilla behaviour as possible while introducing the convenience of pre-selecting items you want to scrap.

The vanilla scrapper logic still runs - your item is sucked out of your inventory, the animation plays, and it spits out your scrap - the mod simply circumvents the selection window if you already have pre-selected items.

Note: **This mod requires both server-side and client-side installation.**

## Usage

The keybinding for marking an item is `[T]` by default and can be configured, see below.

To mark an item, open the scoreboard (`[Tab]`) and mouse hover over the item you want to mark in the top inventory panel, then press the keybinding (`[T]`) to mark.

Repeating the same process on an already marked item will unmark it.

When you activate a scrapper (`[E]` press), it will automatically scrap your marked items one at a time (i.e. one item/stack per button press). If you have no items marked for scrap, it will fall back to the regular scrapper flow and the item selection panel will open.

## Config Options

This mod supports [RiskOfOptions](https://thunderstore.io/package/Rune580/Risk_Of_Options/) for in-game config management.

Currently available config options are:

| Setting                                | Default Value         |
| :------------------------------------- | :-------------------- |
| Toggle item for scrap (keybind)        |          T            |
