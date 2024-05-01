export type Dictionary<TKey extends string | number | symbol, TValue> = {
    [key in TKey]: TValue;
};