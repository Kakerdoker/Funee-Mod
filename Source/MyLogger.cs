using BepInEx.Logging;

namespace FuniPlugin
{
	class MyLogger
	{
		public static ManualLogSource mls = Logger.CreateLogSource("Kakerdoker.Funi");

		public static void Debug(string message)
		{
		#if DEBUG
			mls.LogInfo(message);
		#endif
		}
	}
}
