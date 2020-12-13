function mockLookupDialog(defaultEntityType, additionalEntityTypes, allowMultiSelect, defaultViewId, additionalViewIds) {
    var lookupOptions =
    {
        defaultEntityType,
        entityTypes: [defaultEntityType],
        allowMultiSelect,
        defaultViewId,
        viewIds: [defaultViewId],
    };

    if (additionalEntityTypes) {
        lookupOptions.entityTypes.push(...additionalEntityTypes.split(","))
    }
    if (additionalViewIds) {
        lookupOptions.viewIds.push(...additionalViewIds.split(","));
    }

    Xrm.Utility.lookupObjects(lookupOptions);
}