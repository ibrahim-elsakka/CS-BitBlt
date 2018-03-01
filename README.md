# CS-BitBlt
C# manager for injecting a DLL that handles hooking screenshots in a remote process.

## Usage:

```C#
Pimp pimp = new Pimp(Pimp.BlockMethod.Zero, true, false, false);
if (pimp.Inject("starwarsbattlefrontii")){
    bool check = pimp.IsScreenShot();
//hack routine
}
```

