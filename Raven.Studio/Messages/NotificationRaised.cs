﻿using Raven.Abstractions;

namespace Raven.Studio.Messages
{
	using System;

	public class NotificationRaised
	{
		public NotificationRaised(string message, NotificationLevel level= NotificationLevel.Warning)
		{
			Message = message;
			Level = level;
			CreatedAt = SystemTime.Now;
		}

		public DateTime CreatedAt { get; private set; }
		public string Message { get; private set; }
		public NotificationLevel Level { get; private set; }
	}
}