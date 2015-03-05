using System;
using System.Threading.Tasks;
using RedditSharp;
using System.Collections.Generic;

using HFYBot.Modules;

namespace HFYBot
{
	public class ModuleManager
	{
		/// <summary>
		/// Shared instance of the Reddit API. 
		/// </summary>
		public Reddit RedditAPI;

		/// <summary>
		/// The default subreddit the bot will use when acessing reddit. Some modules may use a different subreddit.
		/// </summary>
		#if DEBUG
		public const string defaultSubreddit = "/r/Bottest";
		#else
		public const string defaultSubreddit = "/r/HFY";
		#endif

		/// <summary>
		/// All modules should be in this list.
		/// </summary>
		static List<Module> modules = new List<Module>();

		public ModuleManager(Reddit reddit){
			Module.moduleManager = this;
			RedditAPI = reddit;
			startInitalModules ();
		}

		/// <summary>
		/// Loads and starts the core modules required form the start. 
		/// </summary>
		public void startInitalModules(){
			modules.Add(new PostReceiverModule(RedditAPI, defaultSubreddit));
			modules.Add (new UserInterfaceModule ());
		}

		/// <summary>
		/// Broadcasts a message to all other modules. This is an asyncronous process, so plan accordingly
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="messageType">The type of message being sent</param>
		/// <param name="messgaeData">Data associated with the message</param>
		public async Task BroadcastMessage(MessageType messageType, Object[] messgaeData){
			//TODO: Implement messgae broadcasting.
		}

		/// <summary>
		/// Generate some UI text for looking nice
		/// </summary>
		/// <returns>The text to be displayed.</returns>
		public string getUIText(){
			string returnText = "";

			modules.TrimExcess ();
			foreach (Module module in modules) {
				returnText += "[" + UserInterfaceModule.ModuleStateToString (module.state) + "] " + module.name + "\n";
			}

			return returnText;
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