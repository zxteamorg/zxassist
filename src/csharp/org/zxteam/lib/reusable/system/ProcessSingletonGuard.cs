namespace org.zxteam.lib.reusable.system
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Security.AccessControl;
	using System.Security.Principal;
	using System.Threading;

	public sealed class ProcessSingletonGuard : IDisposable
	{
		public enum SCOPE
		{
			USER,
			SYSTEM
		}

		/// <summary>
		/// https://msdn.microsoft.com/en-us/library/system.threading.mutex(v=vs.110).aspx
		/// Note
		/// On a server that is running Terminal Services, a named system mutex can have two levels of visibility.
		/// If its name begins with the prefix "Global\", the mutex is visible in all terminal server sessions.
		/// If its name begins with the prefix "Local\", the mutex is visible only in the terminal server session
		/// where it was created. In that case, a separate mutex with the same name can exist in each of the other
		/// terminal server sessions on the server. If you do not specify a prefix when you create a named mutex,
		/// it takes the prefix "Local\". Within a terminal server session, two mutexes whose names differ only by
		/// their prefixes are separate mutexes, and both are visible to all processes in the terminal server session.
		/// That is, the prefix names "Global\" and "Local\" describe the scope of the mutex name relative to terminal
		/// server sessions, not relative to processes.
		/// </summary>
		private const string MUTEX_GLOBAL_PREFIX = @"Global\";
		private const string MUTEX_LOCAL_PREFIX = @"Local\";

		private readonly string _appID;
		public string AppID { get { return this._appID; } }

		private readonly SCOPE _scope;
		public SCOPE Scope { get { return this._scope; } }

		private readonly Mutex _instanceHandle;
		public WaitHandle Handle { get { return this._instanceHandle; } }

		public ProcessSingletonGuard(SCOPE scope = SCOPE.USER, string appID = null)
		{
			if (string.IsNullOrWhiteSpace(appID))
			{
				appID = ResolveAppID();
			}

			this._appID = appID;
			this._scope = scope;

			string mutexName = (this._scope == SCOPE.SYSTEM ? MUTEX_GLOBAL_PREFIX : MUTEX_LOCAL_PREFIX)
				+ this._appID;

			// Add example of setting up security for multi-user usage
			// Work also on localized systems (don't use just "Everyone") 
			MutexAccessRule allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
			MutexSecurity securitySettings = new MutexSecurity();
			securitySettings.AddAccessRule(allowEveryoneRule);

			// Need a place to store a return value in Mutex() constructor call
			bool createdNew;

			this._instanceHandle = new Mutex(false, mutexName, out createdNew, securitySettings);
		}

		public void Dispose()
		{
			try { this._instanceHandle.Dispose(); }
			catch
			{
				/// MS recomendation: 
				/// https://msdn.microsoft.com/en-us/library/system.threading.mutex(v=vs.110).aspx
				/// To dispose of the type directly, call its Dispose method in a try/catch block.
				/* We have not to do here anything... may be logging */
			}
		}

		public bool IsProcessSingleInstance
		{
			get
			{
				try { return this.Handle.WaitOne(TimeSpan.Zero); }
				catch (AbandonedMutexException) { return true; }
			}
		}

		private static string ResolveAppID()
		{
			var attrs = typeof(ProcessSingletonGuard).Assembly.GetCustomAttributes(false);

			{
				GuidAttribute guidAttr = attrs.OfType<GuidAttribute>().FirstOrDefault();
				if (guidAttr != null)
				{
					string value = guidAttr.Value;
					if (!string.IsNullOrWhiteSpace(value)) { return value; }
				}
			}

			{
				AssemblyProductAttribute productAttr = attrs.OfType<AssemblyProductAttribute>().FirstOrDefault();
				if (productAttr != null)
				{
					string value = productAttr.Product;
					if (!string.IsNullOrWhiteSpace(value)) { return value; }
				}
			}

			using (var currentProcess = System.Diagnostics.Process.GetCurrentProcess())
			{
				var currentModuleName = currentProcess.MainModule.ModuleName;
				if (!string.IsNullOrWhiteSpace(currentModuleName)) { return currentModuleName; }
			}

			throw new Exception("Cannot Resolve Application ID");
		}
	}
}