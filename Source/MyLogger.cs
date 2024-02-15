using BepInEx.Logging;

namespace FuneePlugin
{
	class MyLogger
	{
		public static ManualLogSource mls = Logger.CreateLogSource("Kakerdoker.Funi");

		public static void Debug(string message)
		{
			mls.LogInfo(message);
		}

		public static void Error(string message)
		{
			mls.LogError(message);
		}

	}
}
