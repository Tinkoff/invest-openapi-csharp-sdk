using System;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class Defaults
    {
        private DateTimeKind _dateTimeKind = DateTimeKind.Local;

        /// <summary>
        /// Значение DateTimeKind, которое будет взято при передаче в методы API параметра DateTime Unspecified
        /// По умолчанию установлено в DateTimeKind.Local
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public DateTimeKind DateTimeKind
        {
            get => _dateTimeKind;
            set
            {
                if (value == DateTimeKind.Unspecified)
                {
                    throw new InvalidOperationException("Default DateTimeKind must be specified");
                }
                _dateTimeKind = value;
            }
        }
    }
}
