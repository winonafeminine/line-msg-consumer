import { generatedApiUserSvTypes } from './generated-api.user-sv-types';

describe('generatedApiUserSvTypes', () => {
    it('should work', () => {
        expect(generatedApiUserSvTypes()).toEqual(
            'generated-api.user-sv-types'
        );
    });
});
