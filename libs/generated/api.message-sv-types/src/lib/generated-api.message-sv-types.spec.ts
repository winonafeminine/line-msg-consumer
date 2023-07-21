import { generatedApiMessageSvTypes } from "./generated-api.message-sv-types";

describe("generatedApiMessageSvTypes", () => {
  it("should work", () => {
    expect(generatedApiMessageSvTypes()).toEqual(
      "generated-api.message-sv-types"
    );
  });
});
