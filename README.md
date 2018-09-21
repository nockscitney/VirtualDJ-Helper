# VirtualDJ-Helper
A tool which will provide Virtual DJ users with tools and record box management, without using the VirtualDJ software

## Overview
I created this program as a proxy while I'm DJ'ing. Last summer I decided to buy a controller, as I've been DJ'ing for years, which came with a Virtual DJ Limited Edition licence.  While the controller is great, what's not great is not being able to order tracks or view BPM / Key details about tracks while the controller is connected.  As a result, I decided to create a WPF application which would load the library file into the UI for me, so I can use that to navigate tracks while the controller is connected.  Since then, I've had various thoughts and ideas about how I could improve the project further, so I decided to create this GIT and share my project with you all.

## Nuget Packages
The project requires the use of the "Newtonsoft" JSON library, which isn't dstributed with the source.  Once you've pulled the source, simply add the JSON Nuget package to both the Logic Library and VDJ_Helper projects

## Current Version
I am yet to create an "exe" version of the program to download, however you can pull down the project from GIT and compile it yourself should you wish (using Visual Studio)

## Latest Features
(09/21/2018)
- Added in a new section at the bottom which will display your current session history (while the app is open)
- Removed the File Path fields on the track listings and replaces with "Play Count", "First Seen" and "Last Played" fields
- Added "Expanders" to the group headers so it's now possible to minimize groups you don't want to see
- Added a checkbox to filter the track lists and only show tracks that haven't been played
- Added a conversion for the VDJ stored BPM value to show the proper track BPM
