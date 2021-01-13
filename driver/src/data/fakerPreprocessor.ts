import * as faker from 'faker';
import Preprocessor from './preprocessor';
import Record from './record';

export default class FakerPreprocessor extends Preprocessor {
  // eslint-disable-next-line class-methods-use-this
  preprocess(data: Record): Record {
    return FakerPreprocessor.parse(FakerPreprocessor.fake(data));
  }

  private static fake(data: Record): string {
    const fakedData = data;

    Object.keys(data).forEach((key) => {
      const value = fakedData[key];

      if (value === null) {
        return;
      }

      if (Array.isArray(value) && value !== null) {
        fakedData[key] = value.map((arrayItem) => JSON.parse(this.fake(arrayItem as Record)));
      } else if (typeof value === 'object' && value !== null) {
        fakedData[key] = JSON.parse(this.fake(value as Record));
      } else if (typeof value === 'string') {
        fakedData[key] = faker.fake(value);
      }
    });

    return JSON.stringify(fakedData);
  }

  private static parse(data: string): Record {
    const cleansedData = JSON.parse(data);

    Object.keys(cleansedData).forEach((property) => {
      const val = cleansedData[property];

      if (property.endsWith('@faker.number') && typeof val === 'string') {
        cleansedData[property.replace('@faker.number', '')] = this.parseNumber(val, property);
        delete cleansedData[property];
      }

      if (property.endsWith('@faker.datetime') && typeof val === 'string') {
        cleansedData[property.replace('@faker.datetime', '')] = this.parseDate(val, property, false);
        delete cleansedData[property];
      }

      if (property.endsWith('@faker.date') && typeof val === 'string') {
        cleansedData[property.replace('@faker.date', '')] = this.parseDate(val, property, true);
        delete cleansedData[property];
      }

      if (Array.isArray(val)) {
        cleansedData[property] = val.map(
          (collectionRecord) => this.parse(JSON.stringify(collectionRecord)),
        );
      }

      if (typeof val === 'object' && val !== null) {
        cleansedData[property] = this.parse(JSON.stringify(val));
      }
    });

    return cleansedData;
  }

  private static parseNumber(val: string, property: string) {
    const num = +val;
    if (Number.isNaN(num)) {
      throw new Error(`@faker.number syntax failed to convert ${property} with value ${val} to a number.`);
    }

    return num;
  }

  private static parseDate(val: string, property: string, dateOnly: boolean) {
    const parsedDate = Date.parse(val);
    if (Number.isNaN(parsedDate)) {
      throw new Error(`@faker.datetime syntax failed to convert ${property} with value ${val} to a date.`);
    }

    const date = new Date(parsedDate);

    return dateOnly ? date.toISOString().substring(0, 10) : date;
  }
}
