export class ObjectFunctions {

    public static isValid(object: any): boolean {
        if (object === null || object === undefined) {
            return false;
        }
        return true;
    }

    public static isNullOrWhitespace(object: string): boolean {
        if (object === 'null' || object === null || object === undefined || object === '') {
            return true;
        }
        return false;
    }
}
