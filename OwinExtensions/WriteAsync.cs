using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace qed
{
    public static partial class OwinExtensions
    {
        //public static Task WriteAsync(
        //    this IDictionary<string, object> environment, 
        //    string text)
        //{
        //    if (environment == null)
        //        throw new ArgumentNullException("environment");

        //    if (text == null)
        //        throw new ArgumentNullException("text");

        //    return WriteAsync(environment, text, Encoding.UTF8, environment.GetCallCancelled());
        //}

        //public static Task WriteAsync(
        //    this IDictionary<string, object> environment, 
        //    string text, 
        //    Encoding encoding, 
        //    CancellationToken cancel)
        //{
        //    if (environment == null)
        //        throw new ArgumentNullException("environment");
            
        //    if (text == null)
        //        throw new ArgumentNullException("text");
            
        //    if (encoding == null)
        //        throw new ArgumentNullException("encoding");

        //    var buffer = encoding.GetBytes(text);

        //    return WriteAsync(environment, buffer, 0, buffer.Length, cancel);
        //}

        //public static Task WriteAsync(
        //    this IDictionary<string, object> environment, 
        //    byte[] buffer, 
        //    int offset, 
        //    int count, 
        //    CancellationToken cancel)
        //{
        //    if (environment == null)
        //        throw new ArgumentNullException("environment");

        //    if (buffer == null)
        //        throw new ArgumentNullException("buffer");

        //    if (cancel.IsCancellationRequested)
        //    {
        //        var tcs = new TaskCompletionSource<object>();
        //        tcs.TrySetCanceled();
        //        return tcs.Task;
        //    }

        //    var body = environment.GetResponseBody();

        //    if (body == null)
        //        throw new InvalidOperationException("The OWIN response body stream is missing from the environment.");

        //    return body.WriteAsync(buffer, offset, count, cancel);
        //}
    }
}
