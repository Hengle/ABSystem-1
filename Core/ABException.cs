using System;

/*
 * System.Net.WebException: 当无法连接远程服务器或者请求的url返回404, 500等错误码时, WebClient会触发此异常. 其中, 无法连接无法连接服务器时, 底层的WebClient会把原本的SocketException重新抛出为WebException. 所以表面的信息是SocketException, 但是无法catch, 只能catch WebException.
 */

namespace ABSystem
{
    /// <summary>
    /// 当Version相关的json信息无法解析时, 抛出此异常
    /// </summary>
    class ABVersionJsonException : ApplicationException
    {
        public ABVersionJsonException() { }
        public ABVersionJsonException(string message) : base(message) { }
        public ABVersionJsonException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// 当ResourceList相关的json信息无法解析时, 抛出此异常. 
    /// 注意, 即使是空json数组也是合法的, 并不会此异常.
    /// </summary>
    class ABInfoListJsonException : ApplicationException
    {
        public ABInfoListJsonException() { }
        public ABInfoListJsonException(string message) : base(message) { }
        public ABInfoListJsonException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// 当未调用ABManager的Check方法而访问其他属性时, 抛出此异常
    /// </summary>
    class ABUnCheckException : ApplicationException
    {
        public ABUnCheckException() { }
        public ABUnCheckException(string message) : base(message) { }
        public ABUnCheckException(string message, Exception inner) : base(message, inner) { }
    }
}

