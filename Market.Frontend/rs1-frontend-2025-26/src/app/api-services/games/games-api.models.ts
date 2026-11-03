// Interfejsi opisuju JSON koji vraća backend (isti oblik kao C# DTO klase).

export interface GameListItem {
  id: number;
  title: string;
  price: number;
  imageUrl: string | null;
  genre: string | null;
  categoryName: string | null;
  averageRating: number;
  contentType: number;        // 0 = Game, 1 = DLC, 2 = Addon, 3 = Item
  releaseDate: string | null; // C# DateOnly -> "2026-12-01"
  isPreOrder: boolean;        // backend računa: datum izlaska je u budućnosti
}

export interface GameReview {
  id: number;
  text: string | null;
  username: string | null;
  dateCreated: string;
}

// "extends" = nasljeđivanje, kao GameDetailsDto : GameListDto u C#
export interface GameDetails extends GameListItem {
  description: string | null;
  developer: string | null;
  publisher: string | null;
  trailerUrl: string | null;
  systemRequirements: string | null;
  reviews: GameReview[];
}

export interface DashboardStats {
  totalUsers: number;
  totalEarnings: number;
  totalLicensesSold: number;
  activePreOrders: number;
}

export interface Announcement {
  id: number;
  title: string;
  message: string;
  type: string;      // npr. "Sale", "Info", "Maintenance"
  createdAt: string;
}
