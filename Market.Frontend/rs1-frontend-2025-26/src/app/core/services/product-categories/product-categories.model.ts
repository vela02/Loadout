
export class ListProductCategoriesRequest{
  search?: string | null;
  onlyEnabled?: boolean | null;
}

// Za sada komponenta bez paginga može komotno koristiti samo ovo:
export interface ProductCategoryListItem {
  id: number;
  name: string;
  isEnabled: boolean;
}

// Ako jednog dana dodaš paging, dovoljno je promijeniti ovaj alias:
export type ListProductCategoriesResponse = ProductCategoryListItem[];
// ili kasnije: PageResult<ProductCategoryListItem>

// === COMMANDS (WRITE) ===
export interface CreateProductCategoryCommand {
  name?: string | null;
  isEnabled: boolean;
}

export interface UpdateProductCategoryCommand {
  name?: string | null;
  isEnabled: boolean;
}
