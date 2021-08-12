// Copyright 2021, Nitric Technologies Pty Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using Grpc.Core;

﻿namespace Nitric.Api.Common
{
    public abstract class NitricException : Exception
    {
        private static readonly Dictionary<StatusCode, Func<string, NitricException>> Exceptions = new Dictionary<StatusCode, Func<string, NitricException>>
        {
            { StatusCode.Cancelled, (message) => new CancelledException(message) },
            { StatusCode.Unknown, (message) => new UnknownException(message) },
            { StatusCode.InvalidArgument, (message) => new InvalidArgumentException(message) },
            { StatusCode.DeadlineExceeded, (message) => new DeadlineExceededException(message) },
            { StatusCode.NotFound, (message) => new NotFoundException(message) },
            { StatusCode.AlreadyExists, (message) => new AlreadyExistsException(message) },
            { StatusCode.PermissionDenied, (message) => new PermissionDeniedException(message) },
            { StatusCode.ResourceExhausted, (message) => new ResourceExhaustedException(message) },
            { StatusCode.FailedPrecondition, (message) => new FailedPreconditionException(message) },
            { StatusCode.Aborted, (message) => new AbortedException(message) },
            { StatusCode.OutOfRange, (message) => new OutOfRangeException(message) },
            { StatusCode.Unimplemented, (message) => new UnimplementedException(message) },
            { StatusCode.Internal, (message) => new InternalException(message) },
            { StatusCode.Unavailable, (message) => new UnavailableException(message) },
            { StatusCode.DataLoss, (message) => new DataLossException(message) },
            { StatusCode.Unauthenticated,(message) => new UnauthenticatedException(message) },
        };
        public NitricException(string message) : base(message)
        {
        }
        internal static NitricException FromRpcException(RpcException exception)
        {
            return Exceptions.ContainsKey(exception.StatusCode)
                ? Exceptions[exception.StatusCode](exception.Message)
                : new UnknownException(exception.Message);
        }
    }
    public class CancelledException : NitricException
    {
        public CancelledException(string message) : base(message)
        {
        }
    }
    public class UnknownException : NitricException
    {
        public UnknownException(string message) : base(message)
        {
        }
    }
    public class InvalidArgumentException : NitricException
    {
        public InvalidArgumentException(string message) : base(message)
        {
        }
    }
    public class DeadlineExceededException : NitricException
    {
        public DeadlineExceededException(string message) : base(message)
        {
        }
    }
    public class NotFoundException : NitricException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
    public class AlreadyExistsException : NitricException
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
    public class PermissionDeniedException : NitricException
    {
        public PermissionDeniedException(string message) : base(message)
        {
        }
    }
    public class ResourceExhaustedException : NitricException
    {
        public ResourceExhaustedException(string message) : base(message)
        {
        }
    }
    public class FailedPreconditionException : NitricException
    {
        public FailedPreconditionException(string message) : base(message)
        {
        }
    }
    public class AbortedException : NitricException
    {
        public AbortedException(string message) : base(message)
        {
        }
    }
    public class OutOfRangeException : NitricException
    {
        public OutOfRangeException(string message) : base(message)
        {
        }
    }
    public class UnimplementedException : NitricException
    {
        public UnimplementedException(string message) : base(message)
        {
        }
    }
    public class InternalException : NitricException
    {
        public InternalException(string message) : base(message)
        {
        }
    }
    public class UnavailableException : NitricException
    {
        public UnavailableException(string message) : base(message)
        {
        }
    }
    public class DataLossException : NitricException
    {
        public DataLossException(string message) : base(message)
        {
        }
    }
    public class UnauthenticatedException : NitricException
    {
        public UnauthenticatedException(string message) : base(message)
        {
        }
    }
}