﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Network;
using Aura.Shared.Mabi;
using Aura.Shared.Util;
using System;

namespace Aura.Login.Network.Handlers
{
	public partial class LoginServerHandlers : PacketHandlerManager<LoginClient>
	{
		/// <summary>
		/// Sent after connecting, to verify identity.
		/// </summary>
		[PacketHandler(Op.Internal.ServerIdentify)]
		public void Internal_ServerIdentify(LoginClient client, Packet packet)
		{
			var passwordHash = packet.GetString();

			if (!Password.Check(LoginServer.Instance.Conf.Internal.Password, passwordHash))
			{
				Send.Internal_ServerIdentifyR(client, false);

				Log.Warning("Invalid internal password from '{0}'.", client.Address);
				client.Kill();
				return;
			}

			client.State = ClientState.LoggedIn;

			lock (LoginServer.Instance.ChannelClients)
				LoginServer.Instance.ChannelClients.Add(client);

			Send.Internal_ServerIdentifyR(client, true);
		}

		/// <summary>
		/// Regularly sent by channels, to update some numbers.
		/// </summary>
		[PacketHandler(Op.Internal.ChannelStatus)]
		public void Internal_ChannelStatus(LoginClient client, Packet packet)
		{
			if (client.State != ClientState.LoggedIn)
				return;

			var serverName = packet.GetString();
			var channelName = packet.GetString();
			var host = packet.GetString();
			var port = packet.GetInt();
			var users = packet.GetInt();
			var maxUsers = packet.GetInt();

			var server = LoginServer.Instance.ServerList.Add(serverName);

			ChannelInfo channel;
			server.Channels.TryGetValue(channelName, out channel);
			if (channel == null)
			{
				channel = new ChannelInfo(channelName, serverName, host, port);
				server.Channels.Add(channelName, channel);

				Log.Info("New channel registered: {0}", channel.FullName);
			}

			channel.Host = host;
			channel.Port = port;
			channel.Users = users;
			channel.MaxUsers = maxUsers;
			channel.LastUpdate = DateTime.Now;
		}
	}
}