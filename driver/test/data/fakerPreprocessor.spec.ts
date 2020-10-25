import * as faker from 'faker';
import { FakerPreprocessor } from '../../src/data';

describe('FakerPreprocessor', () => {
  let fakerPreprocessor: FakerPreprocessor;
  let fakeSpy: jasmine.Spy<(arg: string) => string>;

  beforeEach(() => {
    fakerPreprocessor = new FakerPreprocessor();
    fakeSpy = spyOn(faker, 'fake');
  });

  describe('.preprocess(data)', () => {
    it('calls faker.fake on data', async () => {
      const data = { foo: 'bar' };
      fakeSpy.and.returnValue('{ "foo": "baz" }');

      fakerPreprocessor.preprocess(data);

      expect(fakeSpy).toHaveBeenCalledWith(JSON.stringify(data));
    });

    it('replaces @faker.number properties with number equivalent', () => {
      const data = {
        'foo@faker.number': '{{random.number}}',
      };
      fakeSpy.and.returnValue('{ "foo@faker.number": "10.50" }');

      const result = fakerPreprocessor.preprocess(data);

      expect(result.foo).toBe(10.50);
      expect(result['foo@faker.number']).toBeUndefined();
    });

    it('throws error when @faker.number value is not a number', () => {
      const data = {
        'foo@faker.number': '{{random.word}}',
      };
      fakeSpy.and.returnValue('{ "foo@faker.number": "baz" }');

      expect(() => fakerPreprocessor.preprocess(data)).toThrowError();
    });

    it('replaces @faker.date properties with YYYY-mm-dd string', () => {
      const data = {
        'foo@faker.date': '{{date.recent}}',
      };
      const date = new Date();
      fakeSpy.and.returnValue(`{ "foo@faker.date": "${date}" }`);

      const result = fakerPreprocessor.preprocess(data);

      expect(result.foo).toBe(date.toISOString().substring(0, 10));
      expect(result['foo@faker.date']).toBeUndefined();
    });

    it('replaces @faker.datetime properties with a date object', () => {
      const data = {
        'foo@faker.datetime': '{{date.recent}}',
      };
      const date = new Date();
      fakeSpy.and.returnValue(`{ "foo@faker.datetime": "${date}" }`);

      const result = fakerPreprocessor.preprocess(data);

      expect(result.foo).toBeInstanceOf(Date);
      expect(result['foo@faker.datetime']).toBeUndefined();
    });

    it('throws error when @faker.date or @faker.datetime value is not a date', () => {
      const data = {
        'foo@faker.date': 'this isn\'t a valid date',
        'bar@faker.datetime': 'this isn\'t a valid datetime',
      };
      fakeSpy.and.returnValue('{ "foo@faker.date": "this isn\'t a valid date", "bar@faker.datetime": "this isn\'t a valid datetime" }');

      expect(() => fakerPreprocessor.preprocess(data)).toThrowError();
    });

    it('processes annotations on nested objects', () => {
      const data = {
        lookup: { 'foo@faker.number': '{{random.number}}' },
      };
      fakeSpy.and.returnValue('{ "lookup": { "foo@faker.number": "10.50" } }');

      const result = fakerPreprocessor.preprocess(data);
      const lookup = result.lookup as any;

      expect(lookup.foo).toBe(10.50);
      expect(lookup['foo@faker.number']).toBeUndefined();
    });

    it('processes annotations on nested arrays', () => {
      const data = {
        relationship: [{ 'foo@faker.number': '{{random.number}}' }],
      };
      fakeSpy.and.returnValue('{ "relationship": [ { "foo@faker.number": "10.50" } ] }');

      const result = fakerPreprocessor.preprocess(data);
      const relatedRecord = (result.relationship as any[])[0];

      expect(relatedRecord.foo).toBe(10.50);
      expect(relatedRecord['foo@faker.number']).toBeUndefined();
    });
  });
});
