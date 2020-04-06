// <copyright file="Speech.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Server.Model.Chat;

namespace COMMO.Server.Data.Models.Structs
{
    public struct Speech
    {
        public SpeechType Type { get; set; }

        public string Receiver { get; set; }

        public string Message { get; set; }

        public ChatChannel ChannelId { get; set; }

    }
}