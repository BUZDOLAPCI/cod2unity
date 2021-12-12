# cod2unity
Call of Duty map loader plugin for Unity.
Goal was practice in reverse engineering, file formats and peripheral Unity systems.

Unity [FPS Sample](https://unity.com/fps-sample) on [Toujane](https://callofduty.fandom.com/wiki/Toujane):

https://user-images.githubusercontent.com/20693893/145734473-21b43146-bec1-47ed-b21b-5c34ed6d5ce1.mp4

# Current status
This Unity plugin can currently load D3DBSP and IWI files. It's only being tested on Call of Duty 2.

Currently this plugin can:
- Load the map geometry
- Extract the materials and textures for the related geometry

![SS1](https://user-images.githubusercontent.com/20693893/145733879-90e9565f-a5a7-4ada-a7b5-22c31895d353.jpg)

![SS2](https://user-images.githubusercontent.com/20693893/145733935-85cf7c63-cfe8-4106-bec5-69174c3e9833.png)

# Usage
- Download the latest .unitypackage release from the releases tab and import into your project. 
- Open up the Setup Window from the Unity's top menu: Cod2Unity > Setup
- Locate your Call of Duty 2 game folder, wait for the plugin to parse game files
- Import any scene from the list to your scene

![Screenshot_1](https://user-images.githubusercontent.com/20693893/145734790-954200df-1b65-460b-b299-b225efae5318.png)
