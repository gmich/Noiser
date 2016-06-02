﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noiser
{
    public static class ResultExtensions
    {
        #region Linq

        public static IEnumerable<TValue> FilterSuccessful<TValue>(this IEnumerable<Result<TValue>> stream)
        {
            return stream.Where(c => c.Success).Select(c => c.Value);
        }
        public static IEnumerable<Result> FilterSuccessful(this IEnumerable<Result> stream)
        {
            return stream.Where(c => c.Success);
        }

        public static Result Enumerate<TValue>(this IEnumerable<Result<TValue>> stream, Action<TValue> transformer)
        {
            foreach(var res in stream)
            {
                if(res.Success)
                {
                    transformer(res.Value);
                }
                else
                {
                    return res;
                }
            }
            return Result.Ok();
        }

        public static bool AnySuccessful(this IEnumerable<Result> stream)
        {
            return stream.Any(c => c.Success);
        }

        public static Result AnyFailures(this IEnumerable<Result> stream)
        {
            var failedResult =  stream.FirstOrDefault(c => c.Failure);
            return failedResult ?? Result.Ok();
        }

        #endregion

        #region Success

        public static Result OnSuccess(this Result result, Func<Result> func)
        {
            if (result.Failure)
            {
                return result;
            }
            return func();
        }

        public static Result<TValue> OnSuccess<TValue>(this Result result, Func<Result<TValue>> func)
        {
            if (result.Failure)
            {
                return Result.FailWith<TValue>(result.State, result.ErrorMessage);
            }
            return func();
        }

        public static Result OnSuccess(this Result result, Action<Result> action)
        {
            if (result.Failure)
            {
                return result;
            }
            action(result);

            return Result.Ok();
        }

        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.Failure)
            {
                return result;
            }
            action();

            return Result.Ok();
        }

        public static Result<TValue> OnSuccess<TValue>(this Result<TValue> result, Action<TValue> action)
        {
            if (result.Failure)
            {
                return result;
            }
            action(result.Value);

            return result;
        }

        public static Result<TNext> OnSuccess<TValue, TNext>(this Result<TValue> result, Func<TValue, Result<TNext>> func)
        {
            if (result.Failure)
            {
                return Result.FailWith<TNext>(result.State, result.ErrorMessage);
            }
            return func(result.Value);
        }

        public static Result OnSuccess<TValue>(this Result<TValue> result, Func<TValue, Result> func)
        {
            if (result.Failure)
            {
                return result;
            }
            return func(result.Value);
        }

        #endregion

        #region Assert

        public static Result<TValue> ThrowIfNull<TValue>(this Result<TValue> result)
        {
            if (result.Success)
            {
                Argument.Require.NotNull(() => result.Value);
            }
            return result;
        }

        #endregion

        #region Failure

        public static Result OnFailure(this Result result, Action<Result> action)
        {
            if (result.Failure)
            {
                action(result);
            }
            return result;
        }

        public static Result<TValue> OnFailure<TValue>(this Result<TValue> result, Action<Result<TValue>> action)
        {
            if (result.Failure)
            {
                action(result);
            }
            return result;
        }

        public static Result<TValue> OnFailure<TValue>(this Result<TValue> result, Func<Result<TValue>> func)
        {
            if (result.Failure)
            {
                return func();
            }
            return result;
        }

        #endregion

        #region Misc

        public static Result<TValue> Log<TValue>(this Result<TValue> result,Action<string> logger,string msg = "")
        {
            logger(msg + result.ErrorMessage);
            return result;
        }

        public static Result Log(this Result result, Action<string> logger, string msg = "")
        {
            logger(msg + result.ErrorMessage);
            return result;
        }

        public static Result<TValue> As<TValue>(this Result result)
        {
            return new Result<TValue>(default(TValue), result.State, result.ErrorMessage);
        }

        public static Result<TNext> As<TValue, TNext>(this Result<TValue> result, Func<TValue, TNext> next)
        {
            return new Result<TNext>(next(result.Value), result.State, result.ErrorMessage);
        }

        public static Result<TNext> As<TNext>(this Result result, Func<TNext> next)
        {
            return new Result<TNext>(next(), result.State, result.ErrorMessage);
        }
        public static Result<TValue> AddErrorMessage<TValue>(this Result<TValue> result, Result other)
        {
            return Result.FailWith(result.Value, result.State, $"{result.ErrorMessage}. {other.ErrorMessage}");
        }

        public static Result<TNewValue> ChangeValue<TValue, TNewValue>(this Result<TValue> result, TNewValue newValue)
        {
            return new Result<TNewValue>(newValue, result.State, result.ErrorMessage);
        }

        public static Result AddErrorMessage(this Result result, Result other)
        {
            return Result.FailWith(result.State, $"{result.ErrorMessage}. {other.ErrorMessage}");
        }

        public static Result<TNext> OnBoth<TValue, TNext>(this Result<TValue> result, Func<Result<TValue>, Result<TNext>> func)
        {
            return func(result.Value);
        }

        public static Result<TValue> OnBoth<TValue>(this Result<TValue> result, Action<TValue> action)
        {
            action(result.Value);
            return result;
        }

        public static Result OnBoth(this Result result, Func<Result> action)
        {
            return action();
        }

        public static Result OnBoth(this Result result, Action action)
        {
            action();
            return result;
        }

        public static Result OnBoth(this Result result, Action<Result> action)
        {
            action(result);
            return result;
        }

        public static Result<TConverted> Cast<TConverted>(this Result result)
        {
            return new Result<TConverted>(default(TConverted), result.State, result.ErrorMessage);
        }

        #endregion

    }
}
