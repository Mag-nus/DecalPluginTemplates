# DecalPluginTemplates

## SamplePlugin-VVS
* This is the sample you want to use if you wish to support the Virindi View System (VVS).
* VVS is the preferred in-game decal plugin renderer.
* This does require that the client have both Decal and VVS installed.
* In my oppinion, this is the preferred way to develop new plugins.

## SamplePlugin
* This is the sample you want to use if you only wish to support the native Decal renderer.
* The native Decal renderer is not as efficient as VVS and has been known to crash clients/decal.
* This requires that the client only have Decal installed.
* In my oppinion, this is an obsolete way to develop new plugins.

## Important Dev Notes
* If you add this plugin to decal and then also create another plugin off of this sample, you will need to change the GUID in
 Properties/AssemblyInfo.cs to have both plugins in decal at the same time.
* In fact, if you use these templates, you should create a unique GUID anyway in
 Properties/AssemblyInfo.cs.