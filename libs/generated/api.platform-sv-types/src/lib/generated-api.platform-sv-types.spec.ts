import { generatedApiPlatformSvTypes } from './generated-api.platform-sv-types';

describe('generatedApiPlatformSvTypes', () => {
    it('should work', () => {
        expect(generatedApiPlatformSvTypes()).toEqual(
            'generated-api.platform-sv-types'
        );
    });
});
