﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Essentials.Core;

namespace NvxEpi
{
    public static class FeedbackFactory
    {
        public static Feedback GetFeedback(Func<bool> func)
        {
            return new BoolFeedback(func);
        }

        public static Feedback GetFeedback(Func<int> func)
        {
            return new IntFeedback(func);
        }

        public static Feedback GetFeedback(Func<string> func)
        {
            return new StringFeedback(func);
        }
    }
}