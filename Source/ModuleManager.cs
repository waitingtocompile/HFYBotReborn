using System;
using System.Threading.Tasks;

namespace HFYBot
{
	public static class ModuleManager
	{
		public static async Task BroadcastMessage(MessageType Message, Object messgaeData){

		}

	}

	/// <summary>
	/// The type of message being sent. messgae types prefixed with an "R" refer to events in the reddit API. This message should imply the type of data being sent.
	/// </summary>
	enum MessageType{
		ModuleStateChange, //For when a module state changes. Should be used for logging purposes as well as the UI
		RPostFound, //For when a new post is received via the API.
		RMailRecieved, //For when a new mail is received wia the reddit API
		MassEditRequired, //For when an event has occured requireing a new mass edit on a specified user.

	}
}