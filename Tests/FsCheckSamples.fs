namespace Ribbanya.Utilities.Tests

open FsCheck
open global.Xunit

module FsCheckSamples =
    module Asdf =
        [<Fact>]
        let Asdfasdf() = Assert.True(true)

        module Asdfasdfasdf =
            [<Fact>]
            let idfk() = Assert.True(true)
