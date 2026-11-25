using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Vertigo.Registration;

namespace Vertigo.Client.UI.Resources;

internal static class AvatarCatalog
{
    public static IReadOnlyList<ProfileAvatar> Avatars { get; } = Array.AsReadOnly(Enum.GetValues<ProfileAvatar>());

    public static ProfileAvatar GetAvatar(int? avatarId)
    {
        return avatarId is int id && Enum.IsDefined((ProfileAvatar)id) ? (ProfileAvatar)id : ProfileAvatar.Skyline;
    }

    public static IImage? Resolve(int? avatarId)
    {
        return Application.Current?.FindResource("Avatar" + GetAvatar(avatarId)) as IImage;
    }
}
