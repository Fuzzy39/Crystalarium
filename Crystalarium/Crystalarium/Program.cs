using Crystalarium.Main;
using CrystalCore.Util;
using CrystalCrash.Main;
using System;


try
{
    using var game = new CrystalGame();
    game.Run();

}
catch (Exception e) when (e is InitializationFailedException or MapLoadException)
{
    // specific error messages are already written in this scenario, so no need to add the generic crash one.

    // initialization failure creates its own sort of 'stacktrace', so we don't need the actual thing.
    bool initFail = e is InitializationFailedException;
    using CrashHandler crash = new(e.Message + (initFail ? "" : "\n" + e.StackTrace));
    crash.Run();
}
catch (Exception e)
{
    using CrashHandler crash = new("Crystalarium unexpectedly crashed." +
                    "\nIf you could report this issue with steps to reproduce it to jmcraft126@gmail.com, that'd be a big help!" +
                    "\nA detailed description of the problem is below:\n\n" + e.Message + "\n" + e.StackTrace);
    crash.Run();
}
