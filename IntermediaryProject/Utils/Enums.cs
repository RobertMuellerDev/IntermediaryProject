namespace IntermediaryProject.Utils {
    public static class Enums {
        public static IEnumerable<T> ToList<T>() where T : IConvertible {
            var type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException($"{type.Name} is not an Enum.");

            var values = Enum.GetValues(type)
                             .Cast<T>()
                             .ToList();

            return values.OrderBy(
                                  value => {
                                      var memInfo = type.GetMember(
                                                                   type.GetEnumName(value) ??
                                                                   throw new InvalidOperationException()
                                                                  );
                                      var orderAttributes = memInfo[0]
                                          .GetCustomAttributes(typeof(EnumOrderAttribute), false);
                                      var order = orderAttributes.Length > 0
                                                      ? ((EnumOrderAttribute)orderAttributes.First()).Order
                                                      : Convert.ToInt32(value) + values.Count;

                                      return order;
                                  }
                                 );
        }
    }
}
