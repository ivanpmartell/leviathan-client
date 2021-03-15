# leviathan-client

This solution will build Assembly-CSharp.dll to let you connect to servers.

Replace in location:

```Leviathan Warships\Leviathan_Data\Managed\Assembly-CSharp.dll```

To debug, use 32-bit dnSpy with Unity 4.0.0 mono. The game was created in Unity 3.5.7, but I do not have the debugging mono.dll for this version.

The modified mono.dll file for debugging purposes can be found in the ```Debugging``` folder. Keep in mind that debugging is not bug-free in dnSpy.