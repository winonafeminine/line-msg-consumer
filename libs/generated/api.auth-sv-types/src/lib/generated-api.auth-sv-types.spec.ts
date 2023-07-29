import { generatedApiAuthSvTypes } from './generated-api.auth-sv-types';

describe('generatedApiAuthSvTypes', () => {
    it('should work', () => {
        expect(generatedApiAuthSvTypes()).toEqual(
            'generated-api.auth-sv-types'
        );
    });
});
