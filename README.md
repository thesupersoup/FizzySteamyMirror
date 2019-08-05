# FizzySteamyMirror

Fizzcube bringing together [Steam](https://store.steampowered.com/) and [Mirror](https://github.com/vis2k/Mirror)

This project was previously called **"SteamNetNetworkTransport"**, this is Version 2 it's a complete rebuild utilising Async of a Steam P2P network transport layer for [Mirror](https://github.com/vis2k/Mirror)

## Quick start - Demo Project 

(coming soon)

## Dependencies
Both of these projects need to be installed and working before you can use this transport.
1. [Facepunch.Steamworks](https://github.com/Facepunch/Facepunch.Steamworks) This variant of FizzySteamyMirror relies on Facepunch.Steamworks to communicate with the [Steamworks API](https://partner.steamgames.com/doc/sdk). **Requires .Net 4.x**
2. [Mirror](https://github.com/vis2k/Mirror) FizzySteamyMirror is also obviously dependant on Mirror which is a streamline, bug fixed, maintained version of UNET for Unity. **Recommended [Stable Version](https://assetstore.unity.com/packages/tools/network/mirror-129321)**

## Setting Up
1. Download and install the dependencies 
2. Download **"FizzySteamyMirror"** and place in your Assets folder somewhere. **If errors occur, open a [Issue ticket.](https://github.com/thesupersoup/FizzySteamyMirror/issues)**

## Building
1. When Building your game you have to place **"steam_appid.txt"** into the dir of the game containing your app's unique AppId.

## Host
1. Open your game through Steam
3. Verify that it shows you as online and playing your game!

**Note: You can run it in Unity as well to verify. You may need to shut down Unity/Steam to "stop" playing, however, due to how Steam checks processes to verify if the game is still running.**

## Play Testing your game locally

1.You need to have both scripts **"Fizzy Steamy Mirror"** and Mirror's **"Telepathy Transport"** on the same GameObject as Mirror's **"NetworkManager"**
2.To test it locally disable **"Fizzy Steamy Mirror"** and enable **"Telepathy Transport"**
3.To test it on Steam P2P again enable **"Fizzy Steamy Mirror"** and disable **"Telepathy Transport"**
