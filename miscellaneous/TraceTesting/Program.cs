// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using TraceTesting;

Trace.AutoFlush = true;

Trace.Listeners.Clear();

Trace.Listeners.Add(new TextWriterTraceListener ("trace.txt"));

var tw = Console.Out;
Trace.Listeners.Add (new TextWriterTraceListener (tw));

Traced.TwoTimes(1);
Traced.TwoTimes(2);
Traced.TwoTimes(0);

Trace.Close();
